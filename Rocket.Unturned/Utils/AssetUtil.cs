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
        /// Gets list of all animal search entries that match search term
        /// </summary>
        public static IEnumerable<SearchEntry> SearchAnimalAssets(string search)
        {
            foreach (var animal in GetAnimalAssets())
            {
                var entry = new SearchEntry(animal, getPriority(animal, search));

                yield return entry;
            }
        }
        /// <summary>
        /// Gets list of all animal assets that match search term
        /// </summary>
        public static IEnumerable<AnimalAsset> GetAnimalAssets(string search)
        {
            foreach (var entry in SearchAnimalAssets(search))
            {
                yield return (AnimalAsset)entry.Asset;
            }
        }
        /// <summary>
        /// Gets most prior search entry
        /// </summary>
        public static AnimalAsset GetAnimalAsset(string search)
        {
            var entry = SearchAnimalAssets(search).OrderByDescending(d => d.Priority).FirstOrDefault();
            if (entry == null || entry.Priority <= 0)
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
        /// Gets list of all item search entries that match search term
        /// </summary>
        public static IEnumerable<SearchEntry> SearchItemAssets(string search)
        {
            foreach (var item in GetItemAssets())
            {
                var entry = new SearchEntry(item, getPriority(item, search));

                yield return entry;
            }
        }
        /// <summary>
        /// Gets list of all item assets that match search term
        /// </summary>
        public static IEnumerable<ItemAsset> GetItemAssets(string search)
        {
            foreach (var entry in SearchItemAssets(search))
            {
                yield return (ItemAsset)entry.Asset;
            }
        }
        /// <summary>
        /// Gets most prior search entry
        /// </summary>
        public static ItemAsset GetItemAsset(string search)
        {
            var entry = SearchItemAssets(search).OrderByDescending(d => d.Priority).FirstOrDefault();
            if (entry == null || entry.Priority <= 0)
                return null;
            return (ItemAsset)entry.Asset;
        }
        #endregion
        #region VEHICLES
        /// <summary>
        /// Gets list of all vehicle assets ordered by ID
        /// </summary>
        public static IEnumerable<VehicleAsset> GetVehicleAssets()
        {
            foreach (var asset in Assets.find(EAssetType.VEHICLE).OrderBy(d => d.id))
            {
                yield return (VehicleAsset)asset;
            }
        }
        /// <summary>
        /// Gets list of all vehicle search entries that match search term
        /// </summary>
        public static IEnumerable<SearchEntry> SearchVehicleAssets(string search)
        {
            foreach (var vehicle in GetVehicleAssets())
            {
                var entry = new SearchEntry(vehicle, getPriority(vehicle, search));

                yield return entry;
            }
        }
        /// <summary>
        /// Gets list of all vehicle assets that match search term
        /// </summary>
        public static IEnumerable<VehicleAsset> GetVehicleAssets(string search)
        {
            foreach (var entry in SearchVehicleAssets(search))
            {
                yield return (VehicleAsset)entry.Asset;
            }
        }
        /// <summary>
        /// Gets most prior search entry
        /// </summary>
        public static VehicleAsset GetVehicleAsset(string search)
        {
            var entry = SearchVehicleAssets(search).OrderByDescending(d => d.Priority).FirstOrDefault();
            if (entry == null || entry.Priority <= 0)
                return null;
            return (VehicleAsset)entry.Asset;
        }
        #endregion
        #region EFFECTS
        /// <summary>
        /// Gets list of all effect assets ordered by ID
        /// </summary>
        public static IEnumerable<EffectAsset> GetEffectAssets()
        {
            foreach (var asset in Assets.find(EAssetType.EFFECT).OrderBy(d => d.id))
            {
                yield return (EffectAsset)asset;
            }
        }
        /// <summary>
        /// Gets list of all effect search entries that match search term
        /// </summary>
        public static IEnumerable<SearchEntry> SearchEffectAssets(string search)
        {
            foreach (var effect in GetEffectAssets())
            {
                var entry = new SearchEntry(effect, getPriority(effect, search));

                yield return entry;
            }
        }
        /// <summary>
        /// Gets list of all effect assets that match search term
        /// </summary>
        public static IEnumerable<EffectAsset> GetEffectAssets(string search)
        {
            foreach (var entry in SearchEffectAssets(search))
            {
                yield return (EffectAsset)entry.Asset;
            }
        }
        /// <summary>
        /// Gets most prior search entry
        /// </summary>
        public static EffectAsset GetEffectAsset(string search)
        {
            var entry = SearchEffectAssets(search).OrderByDescending(d => d.Priority).FirstOrDefault();
            if (entry == null || entry.Priority <= 0)
                return null;
            return (EffectAsset)entry.Asset;
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

            if (U.Settings.Instance.EnableFuzzyComparisonForNames)
            {
                return myFuzzyComparison(asset, search);
            }
            else
            {
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

                int p = 0;

                if (name.ToLower().Contains(search.ToLower()))
                    p++;

                if (asset.name.ToLower().Contains(search.ToLower()))
                    p++;

                return p;
            }
        }

        static int myFuzzyComparison(Asset asset, string search)
        {
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
            var words = asset.name.Split('_').Where(d => d.Length > 0).ToArray();

            foreach (var sWord in wordsSearch)
            {
                if (name.Contains(sWord)) // asset has word in it
                    p++;

                if (name.ToLower().Contains(sWord.ToLower()))
                    p++;

                var sNoSpecial = Regex.Replace(sWord, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);

                foreach (var aWord in wordsAsset.Concat(words))
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

                    var apostropheCount = aWord.Count(c => c == '\'');
                    if (apostropheCount > 0 && apostropheCount % 2 != 0)
                    {
                        string noApostrophe = aWord.Substring(0, aWord.LastIndexOf('\''));

                        if (noApostrophe == sWord) // exact
                            p++;

                        if (noApostrophe.ToLower() == sWord.ToLower()) // ignore case
                            p++;

                        if (name.Contains(noApostrophe))
                            p++;

                        if (name.ToLower().Contains(noApostrophe))
                            p++;
                    }
                }
            }

            return p;
        }
    }
}
