using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GildedRose.Inventory.Domain
{
    public class InventoryItem : Item
    {
        private const int MIN_QUALITY = 0;
        private const int MAX_QUALITY = 50;

        public InventoryItem()
            : base()
        {
            QualityRules = new List<QualityRule>();
        }

        public ItemType Type { get; set; }
        public List<QualityRule> QualityRules { get; set; }

        public void ProcessInventoryItem()
        {
            UpdateSellIn();

            var qualityRule = GetQualityRule();

            UpdateQuality(qualityRule);
        }

        private void UpdateSellIn()
        {
            if (Type != ItemType.Fixed)
                SellIn--;
        }

        private QualityRule GetQualityRule()
        {
            var qualityRule = QualityRules.First(
                r =>
                (r.MinSellIn == null || r.MinSellIn.Value <= SellIn) &&
                (r.MaxSellIn == null || r.MaxSellIn.Value >= SellIn));
            return qualityRule;
        }

        private void UpdateQuality(QualityRule qualityRule)
        {
            if (qualityRule.Adjustment == QualityAdjustment.Increase && Quality < MAX_QUALITY)
                Quality += qualityRule.Rate.Value;
            else if (qualityRule.Adjustment == QualityAdjustment.Decrease && Quality > MIN_QUALITY)
                Quality -= qualityRule.Rate.Value;
            else if (qualityRule.Adjustment == QualityAdjustment.SetToMin)
                Quality = MIN_QUALITY;
        }

        public override string ToString()
        {
            return string.Format("{0} - Quality={1}, SellIn={2}", Name, Quality, SellIn);
        }
    }
}
