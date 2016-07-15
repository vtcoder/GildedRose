using GildedRose.Inventory.Domain;
using System;
using System.Collections.Generic;
using Xunit;

namespace GildedRose.Tests
{
    /// <summary>
    /// Unit tests to cover the InventoryItem class.
    /// </summary>
    public class InventoryItemTests
    {
        private InventoryItem CreateDepreciatingItem(int sellIn, int quality)
        {
            Item item = new InventoryItem { Type = ItemType.Deprecating, Name = "+5 Dexterity Vest", SellIn = sellIn, Quality = quality,
                QualityRules =
                {
                    new QualityRule() { MinSellIn = null, MaxSellIn = -1, Adjustment = QualityAdjustment.Decrease, Rate = 2 },
                    new QualityRule() { MinSellIn = 0, MaxSellIn = null, Adjustment = QualityAdjustment.Decrease, Rate = 1 }
                }
            };
            return item as InventoryItem;
        }

        private InventoryItem CreateDoubleRateDepreciatingItem(int sellIn, int quality)
        {
            Item item = CreateDepreciatingItem(sellIn, quality);
            item.Name = "Conjured Mana Cake";
            foreach (var qualityRule in ((InventoryItem)item).QualityRules)
                qualityRule.Rate *= 2;
            return item as InventoryItem;
        }

        private InventoryItem CreateAppreciatingItem(int sellIn, int quality)
        {
            Item item = new InventoryItem { Type = ItemType.Appreciating, Name = "Aged Brie", SellIn = sellIn, Quality = quality,
                QualityRules =
                {
                    new QualityRule() { MinSellIn = null, MaxSellIn = -1, Adjustment = QualityAdjustment.Increase, Rate = 2 },
                    new QualityRule() { MinSellIn = 0, MaxSellIn = null, Adjustment = QualityAdjustment.Increase, Rate = 1 }
                }
            };
            return item as InventoryItem;
        }

        private InventoryItem CreateAppreciatingItemWithVariableQualityRate(int sellIn, int quality)
        {
            Item item = new InventoryItem { Type = ItemType.AppreciatingTiered, Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = sellIn, Quality = quality,
                QualityRules =
                {
                    new QualityRule() { MinSellIn = null, MaxSellIn = -1, Adjustment = QualityAdjustment.SetToMin, Rate = null },
                    new QualityRule() { MinSellIn = 0, MaxSellIn = 5, Adjustment = QualityAdjustment.Increase, Rate = 3 },
                    new QualityRule() { MinSellIn = 6, MaxSellIn = 10, Adjustment = QualityAdjustment.Increase, Rate = 2 },
                    new QualityRule() { MinSellIn = 11, MaxSellIn = null, Adjustment = QualityAdjustment.Increase, Rate = 1 }
                }
            };
            return item as InventoryItem;
        }

        private InventoryItem CreateFixedQualityItem(int sellIn, int quality)
        {
            Item item = new InventoryItem { Type = ItemType.Fixed, Name = "Sulfuras, Hand of Ragnaros", SellIn = sellIn, Quality = quality,
                QualityRules =
                {
                    new QualityRule() { MinSellIn = null, MaxSellIn = null, Adjustment = QualityAdjustment.None, Rate = null }
                }
            };
            return item as InventoryItem;
        }

        [Fact]
        public void ProcessInventoryItemTest_DepreciatingItem_BeforeSellByDate()
        {
            InventoryItem item = CreateDepreciatingItem(10, 20);

            item.ProcessInventoryItem();
            
            Assert.Equal(9, item.SellIn);
            Assert.Equal(19, item.Quality);
        }

        [Fact]
        public void ProcessInventoryItemTest_DepreciatingItem_BeforeSellByDate_MinQualityReached()
        {
            InventoryItem item = CreateDepreciatingItem(6, 0);
            item.ProcessInventoryItem();

            Assert.Equal(5, item.SellIn);
            Assert.Equal(0, item.Quality);
        }

        [Fact]
        public void ProcessInventoryItemTest_DepreciatingItem_AfterSellByDate()
        {
            InventoryItem item = CreateDepreciatingItem(-1, 20);
            item.ProcessInventoryItem();

            Assert.Equal(-2, item.SellIn);
            Assert.Equal(18, item.Quality);
        }

        [Fact]
        public void ProcessInventoryItemTest_DepreciatingItem_AfterSellByDate_MinQualityReached()
        {
            InventoryItem item = CreateDepreciatingItem(-1, 0);
            item.ProcessInventoryItem();

            Assert.Equal(-2, item.SellIn);
            Assert.Equal(0, item.Quality);
        }

        [Fact]
        public void ProcessInventoryItemTest_DepreciatingItem_DoubleRate_BeforeSellByDate()
        {
            InventoryItem item = CreateDoubleRateDepreciatingItem(10, 20);
            item.ProcessInventoryItem();

            Assert.Equal(9, item.SellIn);
            Assert.Equal(18, item.Quality);
        }

        [Fact]
        public void ProcessInventoryItemTest_DepreciatingItem_DoubleRate_BeforeSellByDate_MinQualityReached()
        {
            InventoryItem item = CreateDoubleRateDepreciatingItem(6, 0);
            item.ProcessInventoryItem();

            Assert.Equal(5, item.SellIn);
            Assert.Equal(0, item.Quality);
        }

        [Fact]
        public void ProcessInventoryItemTest_DepreciatingItem_DoubleRate_AfterSellByDate()
        {
            InventoryItem item = CreateDoubleRateDepreciatingItem(-1, 20);
            item.ProcessInventoryItem();

            Assert.Equal(-2, item.SellIn);
            Assert.Equal(16, item.Quality);
        }

        [Fact]
        public void ProcessInventoryItemTest_DepreciatingItem_DoubleRate_AfterSellByDate_MinQualityReached()
        {
            InventoryItem item = CreateDoubleRateDepreciatingItem(-1, 0);
            item.ProcessInventoryItem();

            Assert.Equal(-2, item.SellIn);
            Assert.Equal(0, item.Quality);
        }

        [Fact]
        public void ProcessInventoryItemTest_AppreciatingItem_BeforeSellByDate()
        {
            InventoryItem item = CreateAppreciatingItem(2, 0);
            item.ProcessInventoryItem();

            Assert.Equal(1, item.SellIn);
            Assert.Equal(1, item.Quality);
        }

        [Fact]
        public void ProcessInventoryItemTest_AppreciatingItem_BeforeSellByDate_MaxQualityReached()
        {
            InventoryItem item = CreateAppreciatingItem(2, 50);
            item.ProcessInventoryItem();

            Assert.Equal(1, item.SellIn);
            Assert.Equal(50, item.Quality);
        }

        [Fact]
        public void ProcessInventoryItemTest_AppreciatingItem_AfterSellByDate()
        {
            InventoryItem item = CreateAppreciatingItem(-1, 0);
            item.ProcessInventoryItem();

            Assert.Equal(-2, item.SellIn);
            Assert.Equal(2, item.Quality);
        }

        [Fact]
        public void ProcessInventoryItemTest_AppreciatingItem_AfterSellByDate_MaxQualityReached()
        {
            InventoryItem item = CreateAppreciatingItem(-1, 50);
            item.ProcessInventoryItem();

            Assert.Equal(-2, item.SellIn);
            Assert.Equal(50, item.Quality);
        }

        [Fact]
        public void ProcessInventoryItemTest_AppreciatingItem_VariableQualityRate_BeforeSellByDate_Tier1()
        {
            InventoryItem item = CreateAppreciatingItemWithVariableQualityRate(15, 20);
            item.ProcessInventoryItem();

            Assert.Equal(14, item.SellIn);
            Assert.Equal(21, item.Quality);
        }

        [Fact]
        public void ProcessInventoryItemTest_AppreciatingItem_VariableQualityRate_BeforeSellByDate_Tier2()
        {
            InventoryItem item = CreateAppreciatingItemWithVariableQualityRate(9, 20);
            item.ProcessInventoryItem();

            Assert.Equal(8, item.SellIn);
            Assert.Equal(22, item.Quality);
        }

        [Fact]
        public void ProcessInventoryItemTest_AppreciatingItem_VariableQualityRate_BeforeSellByDate_Tier3()
        {
            InventoryItem item = CreateAppreciatingItemWithVariableQualityRate(2, 20);
            item.ProcessInventoryItem();

            Assert.Equal(1, item.SellIn);
            Assert.Equal(23, item.Quality);
        }

        [Fact]
        public void ProcessInventoryItemTest_AppreciatingItem_VariableQualityRate_AfterSellByDate()
        {
            InventoryItem item = CreateAppreciatingItemWithVariableQualityRate(-1, 20);
            item.ProcessInventoryItem();

            Assert.Equal(-2, item.SellIn);
            Assert.Equal(0, item.Quality);
        }

        [Fact]
        public void ProcessInventoryItemTest_FixedQualityItem()
        {
            InventoryItem item = CreateFixedQualityItem(0, 80);
            item.ProcessInventoryItem();

            Assert.Equal(0, item.SellIn);
            Assert.Equal(80, item.Quality);
        }
    }
}
