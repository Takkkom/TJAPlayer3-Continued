using System;
using System.Collections.Generic;
using System.Text;

namespace TJAPlayer3
{
	/// <summary>
	/// 「スリーステート」（ON, OFF, 不定 の3状態）を表すアイテム。
	/// </summary>
	internal class ItemThreeState : BaseItem
	{
		// プロパティ

		public ThreeStateType NowThreeState;
		public enum ThreeStateType
		{
			ON,
			OFF,
			Variable
		}


		// コンストラクタ

		public ItemThreeState()
		{
			base.NowItemType = BaseItem.ItemType.ONorOFFor不定スリーステート;
			this.NowThreeState = ThreeStateType.Variable;
		}
		public ItemThreeState( string name, ThreeStateType defaultState )
			: this()
		{
			this.Init( name, defaultState );
		}
		public ItemThreeState(string name, ThreeStateType defaultState, string descriptionJA)
			: this() {
			this.Init(name, defaultState, descriptionJA, descriptionJA);
		}
		public ItemThreeState(string name, ThreeStateType defaultState, string descriptionJA, string descriptionEn)
			: this() {
			this.Init(name, defaultState, descriptionJA, descriptionEn);
		}

		public ItemThreeState( string name, ThreeStateType defaultState, BaseItem.PanelType panelType )
			: this()
		{
			this.Init( name, defaultState, panelType );
		}
		public ItemThreeState(string name, ThreeStateType defaultState, BaseItem.PanelType panelType, string descriptionJA)
			: this() {
			this.Init(name, defaultState, panelType, descriptionJA, descriptionJA);
		}
		public ItemThreeState(string name, ThreeStateType defaultState, BaseItem.PanelType panelType, string descriptionJA, string descriptionEn)
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
			switch( this.NowThreeState )
			{
				case ThreeStateType.ON:
					this.NowThreeState = ThreeStateType.OFF;
					return;

				case ThreeStateType.OFF:
					this.NowThreeState = ThreeStateType.ON;
					return;

				case ThreeStateType.Variable:
					this.NowThreeState = ThreeStateType.ON;
					return;
			}
		}
		public override void MoveItemValuePrev()
		{
			switch( this.NowThreeState )
			{
				case ThreeStateType.ON:
					this.NowThreeState = ThreeStateType.OFF;
					return;

				case ThreeStateType.OFF:
					this.NowThreeState = ThreeStateType.ON;
					return;

				case ThreeStateType.Variable:
					this.NowThreeState = ThreeStateType.OFF;
					return;
			}
		}
		public void Init( string name, ThreeStateType defaultState )
		{
			this.Init( name, defaultState, BaseItem.PanelType.Normal );
		}
		public void Init(string name, ThreeStateType defaultState, string descriptionJA) {
			this.Init(name, defaultState, BaseItem.PanelType.Normal, descriptionJA, descriptionJA);
		}
		public void Init(string name, ThreeStateType defaultState, string descriptionJA, string descriptionEn) {
			this.Init(name, defaultState, BaseItem.PanelType.Normal, descriptionJA, descriptionEn);
		}

		public void Init( string name, ThreeStateType defaultState, BaseItem.PanelType panelType )
		{
			this.Init(name, defaultState, BaseItem.PanelType.Normal, "", "");
		}
		public void Init(string name, ThreeStateType defaultState, BaseItem.PanelType panelType, string descriptionJA) {
			this.Init(name, defaultState, BaseItem.PanelType.Normal, descriptionJA, descriptionJA);
		}
		public void Init(string name, ThreeStateType defaultState, BaseItem.PanelType panelType, string descriptionJA, string descriptionEn) {
			base.Init(name, panelType, descriptionJA, descriptionEn);
			this.NowThreeState = defaultState;
		}
		public override object GetNowValue()
		{
			if ( this.NowThreeState == ThreeStateType.Variable )
			{
				return "- -";
			}
			else
			{
				return this.NowThreeState.ToString();
			}
		}
		public override int GetIndex()
		{
			return (int)this.NowThreeState;
		}
		public override void SetIndex( int index )
		{
		    switch (index )
		    {
		        case 0:
		            this.NowThreeState = ThreeStateType.ON;
		            break;
		        case 1:
		            this.NowThreeState = ThreeStateType.OFF;
		            break;
		        case 2:
		            this.NowThreeState = ThreeStateType.Variable;
		            break;
		        default:
		            throw new ArgumentOutOfRangeException();
		    }
		}
	}
}
