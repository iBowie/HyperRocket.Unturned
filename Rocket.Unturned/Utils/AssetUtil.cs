using SDG.Unturned;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rocket.Unturned.Utils
{
    public static class AssetUtil
    {
        #region ANIMALS
        /// <summary>
        /// Gets list of all animal assets ordered by ID
        /// </summary>
        public static IEnumerable<AnimalAsset> GetAnimalAssets()
        {
            foreach (var asset in Assets.find(EAssetType.ANIMAL).OrderBy(d => d.id))
            {
                yield return (AnimalAsset)asset;
            }
        }
        /// <summary>
        /// Gets list of all animal assets that match search term
        /// </summary>
        public static IEnumerable<SearchEntry> GetAnimalAssets(string search)
        {
            foreach (var animal in GetAnimalAssets())
            {
                var entry = new SearchEntry(animal, getPriority(animal, search));

                yield return entry;
            }
        }
        /// <summary>
        /// Gets most prior search entry
        /// </summary>
        public static AnimalAsset GetAnimalAsset(string search)
        {
            var entry = GetAnimalAssets(search).OrderByDescending(d => d.Priority).FirstOrDefault();
            if (entry == null)
                return null;
            return (AnimalAsset)entry.Asset;
        }
        #endregion
        #region ITEMS
        /// <summary>
        /// Gets list of all item assets ordered by ID
        /// </summary>
        public static IEnumerable<ItemAsset> GetItemAssets()
        {
            foreach (var asset in Assets.find(EAssetType.ITEM).OrderBy(d => d.id))
            {
                yield return (ItemAsset)asset;
            }
        }
        /// <summary>
        /// Gets list of all animal assets that match search term
        /// </summary>
        public static IEnumerable<SearchEntry> GetItemAssets(string search)
        {
            foreach (var item in GetItemAssets())
            {
                var entry = new SearchEntry(item, getPriority(item, search));

                yield return entry;
            }
        }
        /// <summary>
        /// Gets most prior search entry
        /// </summary>
        public static ItemAsset GetItemAsset(string search)
        {
            var entry = GetItemAssets(search).OrderByDescending(d => d.Priority).FirstOrDefault();
            if (entry == null)
                return null;
            return (ItemAsset)entry.Asset;
        }
        #endregion

        public class SearchEntry
        {
            internal SearchEntry(Asset asset, int priority = 0)
            {
                Asset = asset;
                Priority = priority;
            }
            public Asset Asset { get; }
            public int Priority { get; set; }
        }

        static int getPriority(Asset asset, string search)
        {
            if (asset == null)
                return -1;
            int p = 0;

            string name;
            switch (asset)
            {
                case ItemAsset item:
                    name = item.itemName ?? asset.name;
                    break;
                case VehicleAsset vehicle:
                    name = vehicle.vehicleName ?? asset.name;
                    break;
                case AnimalAsset animal:
                    name = animal.animalName ?? asset.name;
                    break;
                default:
                    name = asset.name;
                    break;
            }
            if (name == search) // exact match
                p++;
            if (name.ToLower() == search.ToLower()) // ignore case match
                p++;
            if (name.Contains(search)) // contains
                p++;
            if (name.ToLower().Contains(search.ToLower())) // contains ignore case
                p++;

            var wordsSearch = search.Split(' ').Where(d => d.Length > 0).ToArray();
            var wordsAsset = name.Split(' ').Where(d => d.Length > 0).ToArray();

            foreach (var sWord in wordsSearch)
            {
                if (name.Contains(sWord)) // asset has word in it
                    p++;
                
                var sNoSpecial = Regex.Replace(sWord, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
                
                foreach (var aWord in wordsAsset)
                {
                    if (aWord == sWord) // exact match
                        p++;

                    if (aWord.ToLower() == sWord.ToLower()) // ignore case
                        p++;

                    var aNoSpecial = Regex.Replace(aWord, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
                    
                    if (aNoSpecial == sWord) // exact no symbols match
                        p++;

                    if (aNoSpecial == sNoSpecial) // exact no symbols
                        p++;

                    if (aNoSpecial.ToLower() == sWord.ToLower()) // ignore case no symbols
                        p++;

                    if (aWord.Contains('\''))
                    {
                        string noApostrophe = aWord.Substring(0, aWord.LastIndexOf('\''));

                        if (noApostrophe == sWord) // exact
                            p++;

                        if (noApostrophe.ToLower() == sWord.ToLower()) // ignore case
                            p++;
                    }
                }
            }

            return p;
        }
    }
}
