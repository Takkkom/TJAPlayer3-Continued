using System;
using System.Collections.Generic;
using System.Text;

namespace TJAPlayer3
{
	/// <summary>
	/// 「リスト」（複数の固定値からの１つを選択可能）を表すアイテム。
	/// </summary>
	internal class ItemList : BaseItem
	{
		// プロパティ

		public List<string> SelectedItems;
		public int NowSelectedIndex;


		// コンストラクタ

		public ItemList()
		{
			base.NowItemType = BaseItem.ItemType.List;
			this.NowSelectedIndex = 0;
			this.SelectedItems = new List<string>();
		}
		public ItemList( string name )
			: this()
		{
			this.Init( name );
		}
		public ItemList( string name, BaseItem.PanelType panelType )
			: this()
		{
			this.Init( name, panelType );
		}
		public ItemList( string name, BaseItem.PanelType panelType, int defaultIndex, params string[] items )
			: this()
		{
			this.Init( name, panelType, defaultIndex, items );
		}
		public ItemList(string name, BaseItem.PanelType panelType, int defaultIndex, string descriptionJA, params string[] items)
			: this() {
			this.Init(name, panelType, defaultIndex, descriptionJA, items);
		}
		public ItemList(string name, BaseItem.PanelType panelType, int defaultIndex, string descriptionJA, string descriptionEn, params string[] items)
			: this() {
			this.Init(name, panelType, defaultIndex, descriptionJA, descriptionEn, items);
		}


		// CItemBase 実装

		public override void PressEnter()
		{
			this.MoveItemValueNext();
		}
		public override void MoveItemValueNext()
		{
			if( ++this.NowSelectedIndex >= this.SelectedItems.Count )
			{
				this.NowSelectedIndex = 0;
			}
		}
		public override void MoveItemValuePrev()
		{
			if( --this.NowSelectedIndex < 0 )
			{
				this.NowSelectedIndex = this.SelectedItems.Count - 1;
			}
		}
		public override void Init( string name, BaseItem.PanelType panelType )
		{
			base.Init( name, panelType );
			this.NowSelectedIndex = 0;
			this.SelectedItems.Clear();
		}
		public void Init( string name, BaseItem.PanelType panelType, int defaultIndex, params string[] items )
		{
			this.Init(name, panelType, defaultIndex, "", "",items);
		}
		public void Init(string name, BaseItem.PanelType panelType, int defaultIndex, string descriptionJA, params string[] items) {
			this.Init(name, panelType, defaultIndex, descriptionJA, descriptionJA, items);
		}
		public void Init(string name, BaseItem.PanelType panelType, int defaultIndex, string descriptionJA, string descriptionEn, params string[] items) {
			base.Init(name, panelType, descriptionJA, descriptionEn);
			this.NowSelectedIndex = defaultIndex;
			foreach (string str in items) {
				this.SelectedItems.Add(str);
			}
		}
		public override object GetNowValue()
		{
			return this.SelectedItems[ NowSelectedIndex ];
		}
		public override int GetIndex()
		{
			return NowSelectedIndex;
		}
		public override void SetIndex( int index )
		{
			NowSelectedIndex = index;
		}
	}




	/// <summary>
	/// 簡易コンフィグの「切り替え」に使用する、「リスト」（複数の固定値からの１つを選択可能）を表すアイテム。
	/// e種別が違うのと、tEnter押下()で何もしない以外は、「リスト」そのまま。
	/// </summary>
	internal class CSwitchItemList : ItemList
	{
		// コンストラクタ

		public CSwitchItemList()
		{
			base.NowItemType = BaseItem.ItemType.SwitchingList;
			this.NowSelectedIndex = 0;
			this.SelectedItems = new List<string>();
		}
		public CSwitchItemList( string name )
			: this()
		{
			this.Init( name );
		}
		public CSwitchItemList( string name, BaseItem.PanelType panelType )
			: this()
		{
			this.Init( name, panelType );
		}
		public CSwitchItemList( string name, BaseItem.PanelType panelType, int defaultIndex, params string[] items )
			: this()
		{
			this.Init( name, panelType, defaultIndex, items );
		}
		public CSwitchItemList(string name, BaseItem.PanelType panelType, int defaultIndex, string descriptionJA, params string[] items)
			: this() {
			this.Init(name, panelType, defaultIndex, descriptionJA, items);
		}
		public CSwitchItemList( string name, BaseItem.PanelType panelType, int defaultIndex, string descriptionJA, string descriptionEn, params string[] items )
			: this()
		{
			this.Init( name, panelType, defaultIndex, descriptionJA, descriptionEn, items );
		}

		public override void PressEnter()
		{
			// this.t項目値を次へ移動();	// 何もしない
		}
	}

}
