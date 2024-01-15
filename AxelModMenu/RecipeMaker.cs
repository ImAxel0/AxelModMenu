using SonsAxLib;
using SonsSdk;
using SUI;
using Sons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sons.Items.Core;

namespace AxelModMenu;

public class RecipeMaker
{
    public static Observable<string> Ingredients = new("");
    public static Observable<string> ObtainedItemId = new("");

    public static void AddCustomRecipe()
    {
        string sanitizedIngredients = Ingredients.Value.Replace(" ", "");
        string sanitizedObtained = ObtainedItemId.Value.Replace(" ", "");

        char[] validChars = "1234567890+x".ToCharArray();
        char[] validChars2 = "1234567890".ToCharArray();
        foreach (char ch in sanitizedIngredients)
        {
            if (!validChars.Contains(ch))
            {
                SonsTools.ShowMessage("<color=red>Error</color>, invalid character found", 5);
                return;
            }
        }
        foreach (char ch in sanitizedObtained)
        {
            if (!validChars2.Contains(ch))
            {
                SonsTools.ShowMessage("<color=red>Error</color>, invalid character found", 5);
                return;
            }
        }

        Dictionary<ItemIdManager.ItemsId, int> idCountPair = new();
        string[] ingredients = sanitizedIngredients.Split('+', StringSplitOptions.None);
        foreach (var ingredient in ingredients)
        {
            string[] ingredientInfo = ingredient.Split('x', StringSplitOptions.None);
            if (int.TryParse(ingredientInfo[0], out int id))
            {
                ItemIdManager.ItemsId itemId = (ItemIdManager.ItemsId)id;
                if (int.TryParse(ingredientInfo[1], out int count))
                {
                    idCountPair.Add(itemId, count);
                }
                else SonsTools.ShowMessage("<color=red>Wrong value</color> in ingredients counts");
            }
            else SonsTools.ShowMessage("<color=red>Wrong value</color> in ingredients id's");
        }

        if (int.TryParse(sanitizedObtained, out int obtainedId))
        {
            if (CustomRecipes.CreateRecipe("CustomRecipe", idCountPair, (ItemIdManager.ItemsId)obtainedId))
            {
                SonsTools.ShowMessage($"Reciped added for <color=green>{ItemDatabaseManager.ItemById(obtainedId).Name}</color>");
            }
        }
        else SonsTools.ShowMessage("<color=red>Couldn't</color> add recipe");
    }
}
