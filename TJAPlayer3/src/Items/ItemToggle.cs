using System;
using System.Collections.Generic;
using System.Text;

namespace TJAPlayer3
{
	/// <summary>
	/// 「トグル」（ON, OFF の2状態）を表すアイテム。
	/// </summary>
	internal class ItemToggle : BaseItem
	{
		// プロパティ

		public bool IsOn;

		
		// コンストラクタ

		public ItemToggle()
		{
			base.NowItemType = BaseItem.ItemType.ONorOFFToggle;
			this.IsOn = false;
		}
		public ItemToggle( string name, bool defaultState )
			: this()
		{
			this.Init( name, defaultState );
		}
		public ItemToggle(string name, bool defaultState, string descriptionJA)
			: this() {
			this.Init(name, defaultState, descriptionJA);
		}
		public ItemToggle(string name, bool defaultState, string descriptionJA, string descriptionEn)
			: this() {
			this.Init(name, defaultState, descriptionJA, descriptionEn);
		}
		public ItemToggle(string name, bool defaultState, BaseItem.PanelType panelType)
			: this()
		{
			this.Init( name, defaultState, panelType );
		}
		public ItemToggle(string name, bool defaultState, BaseItem.PanelType panelType, string descriptionJA)
			: this() {
			this.Init(name, defaultState, panelType, descriptionJA);
		}
		public ItemToggle(string name, bool defaultState, BaseItem.PanelType panelType, string descriptionJA, string descriptionEn)
			: this() {
			this.Init(name, defaultState, panelType, descriptionJA, descriptionEn);
		}


		// CItemBase 実装

		public override void PressEnter()
		{
			this.MoveItemValueNext();
		}
		public override void MoveItemValueNext()
		{
			this.IsOn = !this.IsOn;
		}
		public override void MoveItemValuePrev()
		{
			this.MoveItemValueNext();
		}
		public void Init( string name, bool defaultState )
		{
			this.Init( name, defaultState, BaseItem.PanelType.Normal );
		}
		public void Init(string name, bool defaultState, string descriptionJA) {
			this.Init(name, defaultState, BaseItem.PanelType.Normal, descriptionJA, descriptionJA);
		}
		public void Init(string name, bool defaultState, string descriptionJA, string descriptionEn) {
			this.Init(name, defaultState, BaseItem.PanelType.Normal, descriptionJA, descriptionEn);
		}

		public void Init(string name, bool defaultState, BaseItem.PanelType panelType)
		{
			this.Init(name, defaultState, panelType, "", "");
		}
		public void Init(string name, bool defaultState, BaseItem.PanelType panelType, string descriptionJA) {
			this.Init(name, defaultState, panelType, descriptionJA, descriptionJA);
		}
		public void Init(string name, bool defaultState, BaseItem.PanelType panelType, string descriptionJA, string descriptionEn) {
			base.Init(name, panelType, descriptionJA, descriptionEn);
			this.IsOn = defaultState;
		}
		public override object GetNowValue()
		{
			return ( this.IsOn ) ? "ON" : "OFF";
		}
		public override int GetIndex()
		{
			return ( this.IsOn ) ? 1 : 0;
		}
		public override void SetIndex( int index )
		{
			switch ( index )
			{
				case 0:
					this.IsOn = false;
					break;
				case 1:
					this.IsOn = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
