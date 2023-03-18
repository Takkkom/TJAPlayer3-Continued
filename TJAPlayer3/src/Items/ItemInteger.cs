using System;
using System.Collections.Generic;
using System.Text;

namespace TJAPlayer3
{
	/// <summary>
	/// 「整数」を表すアイテム。
	/// </summary>
	internal class ItemInteger : BaseItem
	{
		// プロパティ

		public int NowValue;
		public bool ValueInFocus;


		// コンストラクタ

		public ItemInteger()
		{
			base.NowItemType = BaseItem.ItemType.Integer;
			this.MinValue = 0;
			this.MaxValue = 0;
			this.NowValue = 0;
			this.ValueInFocus = false;
		}
		public ItemInteger( string name, int minValue, int maxValue, int defaultValue )
			: this()
		{
			this.Init( name, minValue, maxValue, defaultValue );
		}
		public ItemInteger(string name, int minValue, int maxValue, int defaultValue, string descriptionJA)
			: this() {
			this.Init(name, minValue, maxValue, defaultValue, descriptionJA);
		}
		public ItemInteger(string name, int minValue, int maxValue, int defaultValue, string descriptionJA, string descriptionEn)
			: this() {
			this.Init(name, minValue, maxValue, defaultValue, descriptionJA, descriptionEn);
		}

	
		public ItemInteger( string name, int minValue, int maxValue, int defaultValue, BaseItem.PanelType panelType )
			: this()
		{
			this.Init( name, minValue, maxValue, defaultValue, panelType );
		}
		public ItemInteger(string name, int minValue, int maxValue, int defaultValue, BaseItem.PanelType panelType, string descriptionJA)
			: this() {
			this.Init(name, minValue, maxValue, defaultValue, panelType, descriptionJA);
		}
		public ItemInteger(string name, int minValue, int maxValue, int defaultValue, BaseItem.PanelType panelType, string descriptionJA, string descriptionEn)
			: this() {
			this.Init(name, minValue, maxValue, defaultValue, panelType, descriptionJA, descriptionEn);
		}


		// CItemBase 実装

		public override void PressEnter()
		{
			this.ValueInFocus = !this.ValueInFocus;
		}
		public override void MoveItemValueNext()
		{
			if( ++this.NowValue > this.MaxValue )
			{
				this.NowValue = this.MaxValue;
			}
		}
		public override void MoveItemValuePrev()
		{
			if( --this.NowValue < this.MinValue )
			{
				this.NowValue = this.MinValue;
			}
		}
		public void Init( string name, int minValue, int maxValue, int defaultValue )
		{
			this.Init( name, minValue, maxValue, defaultValue, BaseItem.PanelType.Normal, "", "" );
		}
		public void Init(string name, int minValue, int maxValue, int defaultValue, string descriptionJA) {
			this.Init(name, minValue, maxValue, defaultValue, BaseItem.PanelType.Normal, descriptionJA, descriptionJA);
		}
		public void Init(string name, int minValue, int maxValue, int defaultValue, string descriptionJA, string descriptionEn) {
			this.Init(name, minValue, maxValue, defaultValue, BaseItem.PanelType.Normal, descriptionJA, descriptionEn);
		}

	
		public void Init( string name, int minValue, int maxValue, int defaultValue, BaseItem.PanelType panelType )
		{
			this.Init( name, minValue, maxValue, defaultValue, panelType, "", "" );
		}
		public void Init(string name, int minValue, int maxValue, int defaultValue, BaseItem.PanelType panelType, string descriptionJA) {
			this.Init(name, minValue, maxValue, defaultValue, panelType, descriptionJA, descriptionJA);
		}
		public void Init(string name, int minValue, int maxValue, int defaultValue, BaseItem.PanelType panelType, string descriptionJA, string descriptionEn) {
			base.Init(name, panelType, descriptionJA, descriptionEn);
			this.MinValue = minValue;
			this.MaxValue = maxValue;
			this.NowValue = defaultValue;
			this.ValueInFocus = false;
		}
		public override object GetNowValue()
		{
			return this.NowValue;
		}
		public override int GetIndex()
		{
			return this.NowValue;
		}
		public override void SetIndex( int index )
		{
			this.NowValue = index;
		}
		// その他

		#region [ private ]
		//-----------------
		private int MinValue;
		private int MaxValue;
		//-----------------
		#endregion
	}
}
