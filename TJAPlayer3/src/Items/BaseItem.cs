using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace TJAPlayer3
{
	//超絶便利じゃん! なんで今までこれの存在に気づかなかったんだ...

	/// <summary>
	/// すべてのアイテムの基本クラス。
	/// </summary>
	internal class BaseItem
	{
		// プロパティ

		public PanelType NowPanelType;
		public enum PanelType
		{
			Normal,
			Other
		}

		public ItemType NowItemType;
		public enum ItemType
		{
			Normal,
			ONorOFFToggle,
			ONorOFFor不定スリーステート,
			Integer,
			List,
			SwitchingList
		}

		public string Name;
		public string Description;


		// コンストラクタ

		public BaseItem()
		{
			this.Name = "";
			this.Description = "";
		}
		public BaseItem( string name )
			: this()
		{
			this.Init( name );
		}
		public BaseItem(string name, string descriptionJA)
			: this() {
			this.Init(name, descriptionJA);
		}
		public BaseItem(string name,  string descriptionJA, string descriptionEn)
			: this() {
			this.Init(name, descriptionJA, descriptionEn);
		}

		public BaseItem(string name, PanelType panelType)
			: this()
		{
			this.Init( name, panelType );
		}
		public BaseItem(string name, PanelType panelType, string descriptionJA)
			: this() {
			this.Init(name, panelType, descriptionJA);
		}
		public BaseItem(string name, PanelType panelType, string descriptionJA, string descriptionEn)
			: this() {
			this.Init(name, panelType, descriptionJA, descriptionEn);
		}

		
		// メソッド；子クラスで実装する

		public virtual void PressEnter()
		{
		}
		public virtual void MoveItemValueNext()
		{
		}
		public virtual void MoveItemValuePrev()
		{
		}
		public virtual void Init( string name )
		{
			this.Init( name, PanelType.Normal );
		}
		public virtual void Init(string name, string descriptionJA) {
			this.Init(name, PanelType.Normal, descriptionJA, descriptionJA);
		}
		public virtual void Init(string name, string descriptionJA, string descriptionEn) {
			this.Init(name, PanelType.Normal, descriptionJA, descriptionEn);
		}

		public virtual void Init( string name, PanelType panelType )
		{
			this.Init(name, panelType, "", "");
		}
		public virtual void Init(string name, PanelType panelType, string descriptionJA) {
			this.Init(name, panelType, descriptionJA, descriptionJA);
		}
		public virtual void Init(string name, PanelType panelType, string descriptionJA, string descriptionEn) {
			this.Name = name;
			this.NowPanelType = panelType;
			this.Description = (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ja") ? descriptionJA : descriptionEn;
		}
		public virtual object GetNowValue()
		{
			return null;
		}
		public virtual int GetIndex()
		{
			return 0;
		}
		public virtual void SetIndex( int index )
		{
		}
	}
}
