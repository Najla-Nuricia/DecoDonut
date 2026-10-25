using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DonutDataGeneratorWindow : EditorWindow
{
    private const int MaxToppingsPerOrder = 6;
    private const string DefaultOrderFolder = "Assets/_dev/SO/Orders/Generated";
    private const string DefaultCustomerFolder = "Assets/_dev/SO/Customers/Generated";

    private int orderCount = 10;
    private int customerCount = 10;
    private int minimumToppings;
    private int maximumToppings = MaxToppingsPerOrder;
    private int minimumReward = 10;
    private int maximumReward = 50;
    private string orderFolder = DefaultOrderFolder;
    private string customerFolder = DefaultCustomerFolder;
    private int randomSeed;

    [MenuItem("DecoDonut/Generate Donut Orders & Customers")]
    private static void Open()
    {
        GetWindow<DonutDataGeneratorWindow>("Donut Data Generator");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Generation Settings", EditorStyles.boldLabel);
        orderCount = Mathf.Max(1, EditorGUILayout.IntField("Order Count", orderCount));
        customerCount = Mathf.Max(1, EditorGUILayout.IntField("Customer Count", customerCount));
        minimumToppings = Mathf.Clamp(
            EditorGUILayout.IntField("Minimum Toppings", minimumToppings),
            0,
            MaxToppingsPerOrder);
        maximumToppings = Mathf.Clamp(
            EditorGUILayout.IntField("Maximum Toppings", maximumToppings),
            minimumToppings,
            MaxToppingsPerOrder);
        minimumReward = Mathf.Max(0, EditorGUILayout.IntField("Minimum Reward", minimumReward));
        maximumReward = Mathf.Max(minimumReward, EditorGUILayout.IntField("Maximum Reward", maximumReward));
        randomSeed = EditorGUILayout.IntField("Random Seed (0 = random)", randomSeed);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Output Folders", EditorStyles.boldLabel);
        orderFolder = EditorGUILayout.TextField("Orders", orderFolder);
        customerFolder = EditorGUILayout.TextField("Customers", customerFolder);

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "All IngredientSO assets are scanned. Ingredients whose name contains " +
            "\"Donut\" become bases; all other ingredients are treated as toppings. " +
            "Existing assets are never overwritten.",
            MessageType.Info);

        GUI.enabled = !Application.isPlaying;
        if (GUILayout.Button("Generate"))
            Generate();
        GUI.enabled = true;
    }

    private void Generate()
    {
        if (!TryGetIngredients(out List<IngredientSO> donuts, out List<IngredientSO> toppings))
            return;

        if (!TryPrepareFolder(orderFolder, "order") || !TryPrepareFolder(customerFolder, "customer"))
            return;

        List<IngredientSO[]> recipes = BuildRecipes(donuts, toppings);
        if (recipes.Count < orderCount)
        {
            bool continueWithDuplicates = EditorUtility.DisplayDialog(
                "Not Enough Unique Combinations",
                $"Only {recipes.Count} unique order combinations are available for {orderCount} requested orders. " +
                "Generate the available unique combinations and then reuse combinations as needed?",
                "Continue",
                "Cancel");

            if (!continueWithDuplicates)
                return;
        }

        if (HasExistingGeneratedAssets(orderFolder, customerFolder))
        {
            bool continueGeneration = EditorUtility.DisplayDialog(
                "Existing Generated Assets",
                "One or more generated asset names already exist. They will not be overwritten; " +
                "Unity will create unique names. Continue?",
                "Continue",
                "Cancel");

            if (!continueGeneration)
                return;
        }

        System.Random random = randomSeed == 0 ? new System.Random() : new System.Random(randomSeed);
        List<OrderSO> orders = CreateOrders(recipes, random);
        CreateCustomers(orders, random);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "Generation Complete",
            $"Created {orders.Count} orders and {customerCount} customers.",
            "OK");
    }

    private bool TryGetIngredients(
        out List<IngredientSO> donuts,
        out List<IngredientSO> toppings)
    {
        donuts = new List<IngredientSO>();
        toppings = new List<IngredientSO>();

        string[] ingredientGuids = AssetDatabase.FindAssets("t:IngredientSO");
        foreach (string guid in ingredientGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            IngredientSO ingredient = AssetDatabase.LoadAssetAtPath<IngredientSO>(path);
            if (ingredient == null)
                continue;

            string ingredientName = ingredient.ingredientName ?? string.Empty;
            if (ingredientName.IndexOf("Donut", StringComparison.OrdinalIgnoreCase) >= 0)
                donuts.Add(ingredient);
            else
                toppings.Add(ingredient);
        }

        if (donuts.Count == 0)
        {
            EditorUtility.DisplayDialog(
                "Cannot Generate",
                "No IngredientSO with a name containing \"Donut\" was found.",
                "OK");
            return false;
        }

        if (minimumToppings > toppings.Count)
        {
            EditorUtility.DisplayDialog(
                "Cannot Generate",
                $"Minimum toppings ({minimumToppings}) exceeds the {toppings.Count} available topping ingredients.",
                "OK");
            return false;
        }

        return true;
    }

    private List<IngredientSO[]> BuildRecipes(
        List<IngredientSO> donuts,
        List<IngredientSO> toppings)
    {
        System.Random random = randomSeed == 0 ? new System.Random() : new System.Random(randomSeed);
        List<IngredientSO[]> recipes = new List<IngredientSO[]>();
        HashSet<string> combinations = new HashSet<string>();
        int possibleToppingCombinations = CountToppingCombinations(
            toppings.Count,
            minimumToppings,
            maximumToppings);
        int targetUniqueRecipes = Mathf.Min(orderCount, donuts.Count * possibleToppingCombinations);
        int attempts = Mathf.Max(100, targetUniqueRecipes * 50);

        while (recipes.Count < targetUniqueRecipes && attempts-- > 0)
        {
            IngredientSO donut = donuts[random.Next(donuts.Count)];
            int toppingCount = random.Next(minimumToppings, maximumToppings + 1);
            List<IngredientSO> selectedToppings = PickDistinct(toppings, toppingCount, random);
            selectedToppings.Sort((left, right) =>
                string.Compare(GetAssetGuid(left), GetAssetGuid(right), StringComparison.Ordinal));

            IngredientSO[] recipe = new IngredientSO[1 + selectedToppings.Count];
            recipe[0] = donut;
            selectedToppings.CopyTo(recipe, 1);

            string key = string.Join("|", recipe.Select(GetAssetGuid));
            if (combinations.Add(key))
                recipes.Add(recipe);
        }

        return recipes;
    }

    private List<OrderSO> CreateOrders(List<IngredientSO[]> recipes, System.Random random)
    {
        List<OrderSO> orders = new List<OrderSO>();
        for (int index = 0; index < orderCount; index++)
        {
            IngredientSO[] recipe = recipes[index % recipes.Count];
            string assetName = $"GeneratedOrder_{index + 1:00}";
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(
                $"{orderFolder.TrimEnd('/')}/{assetName}.asset");

            OrderSO order = CreateInstance<OrderSO>();
            order.orderName = $"Order {index + 1:00} - {recipe[0].ingredientName}";
            order.donutImage = recipe[0].ingredientImage;
            order.recipeIngredients = new List<IngredientSO>(recipe);
            order.rewardCoins = random.Next(minimumReward, maximumReward + 1);
            AssetDatabase.CreateAsset(order, assetPath);
            orders.Add(order);
        }

        return orders;
    }

    private void CreateCustomers(List<OrderSO> orders, System.Random random)
    {
        for (int index = 0; index < customerCount; index++)
        {
            string assetName = $"GeneratedCustomer_{index + 1:00}";
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(
                $"{customerFolder.TrimEnd('/')}/{assetName}.asset");

            CustomerSO customer = CreateInstance<CustomerSO>();
            customer.customerName = $"Customer {index + 1:00}";
            customer.customerOrder = orders[random.Next(orders.Count)];
            AssetDatabase.CreateAsset(customer, assetPath);
        }
    }

    private bool TryPrepareFolder(string folder, string assetType)
    {
        if (string.IsNullOrWhiteSpace(folder) ||
            !folder.StartsWith("Assets", StringComparison.Ordinal) ||
            Path.IsPathRooted(folder))
        {
            EditorUtility.DisplayDialog(
                "Invalid Output Folder",
                $"The {assetType} folder must be a project-relative path beginning with 'Assets'.",
                "OK");
            return false;
        }

        EnsureFolderExists(folder);
        return true;
    }

    private bool HasExistingGeneratedAssets(string ordersPath, string customersPath)
    {
        return AssetDatabase.FindAssets("GeneratedOrder_", new[] { ordersPath }).Length > 0 ||
               AssetDatabase.FindAssets("GeneratedCustomer_", new[] { customersPath }).Length > 0;
    }

    private static void EnsureFolderExists(string folder)
    {
        string[] parts = folder.Split('/');
        string current = parts[0];
        for (int index = 1; index < parts.Length; index++)
        {
            string next = $"{current}/{parts[index]}";
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[index]);
            current = next;
        }
    }

    private static List<IngredientSO> PickDistinct(
        List<IngredientSO> source,
        int count,
        System.Random random)
    {
        List<IngredientSO> shuffled = new List<IngredientSO>(source);
        for (int index = shuffled.Count - 1; index > 0; index--)
        {
            int swapIndex = random.Next(index + 1);
            IngredientSO value = shuffled[index];
            shuffled[index] = shuffled[swapIndex];
            shuffled[swapIndex] = value;
        }

        return shuffled.Take(count).ToList();
    }

    private static int CountToppingCombinations(int toppingCount, int minimum, int maximum)
    {
        long total = 0;
        for (int count = minimum; count <= Mathf.Min(maximum, toppingCount); count++)
            total += Binomial(toppingCount, count);
        return total > int.MaxValue ? int.MaxValue : (int)total;
    }

    private static long Binomial(int n, int k)
    {
        k = Mathf.Min(k, n - k);
        long result = 1;
        for (int index = 1; index <= k; index++)
            result = result * (n - k + index) / index;
        return result;
    }

    private static string GetAssetGuid(IngredientSO ingredient)
    {
        return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(ingredient));
    }
}
