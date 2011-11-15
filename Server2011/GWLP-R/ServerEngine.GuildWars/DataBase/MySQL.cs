// 
//  ____  _     __  __      _        _ 
// |  _ \| |__ |  \/  | ___| |_ __ _| |
// | | | | '_ \| |\/| |/ _ \ __/ _` | |
// | |_| | |_) | |  | |  __/ || (_| | |
// |____/|_.__/|_|  |_|\___|\__\__,_|_|
//
// Auto-generated from gwlpr on 2011-11-15 14:53:44Z.
// Please visit http://code.google.com/p/dblinq2007/ for more information.
//
namespace ServerEngine.GuildWars.DataBase
{
	using System;
	using System.ComponentModel;
	using System.Data;
#if MONO_STRICT
	using System.Data.Linq;
#else   // MONO_STRICT
	using DbLinq.Data.Linq;
	using DbLinq.Vendor;
#endif  // MONO_STRICT
	using System.Data.Linq.Mapping;
	using System.Diagnostics;
	
	
	public partial class MySQL : DataContext
	{
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		#endregion
		
		
		public MySQL(string connectionString) : 
				base(connectionString)
		{
			this.OnCreated();
		}
		
		public MySQL(string connection, MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			this.OnCreated();
		}
		
		public MySQL(IDbConnection connection, MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			this.OnCreated();
		}
		
		public Table<accountsMasterData> accountsMasterData
		{
			get
			{
				return this.GetTable<accountsMasterData>();
			}
		}
		
		public Table<charsMasterData> charsMasterData
		{
			get
			{
				return this.GetTable<charsMasterData>();
			}
		}
		
		public Table<groupsCommands> groupsCommands
		{
			get
			{
				return this.GetTable<groupsCommands>();
			}
		}
		
		public Table<groupsMasterData> groupsMasterData
		{
			get
			{
				return this.GetTable<groupsMasterData>();
			}
		}
		
		public Table<itemsMasterData> itemsMasterData
		{
			get
			{
				return this.GetTable<itemsMasterData>();
			}
		}
		
		public Table<itemsPerSonALData> itemsPerSonALData
		{
			get
			{
				return this.GetTable<itemsPerSonALData>();
			}
		}
		
		public Table<itemsPredefinedData> itemsPredefinedData
		{
			get
			{
				return this.GetTable<itemsPredefinedData>();
			}
		}
		
		public Table<mapsMasterData> mapsMasterData
		{
			get
			{
				return this.GetTable<mapsMasterData>();
			}
		}
		
		public Table<mapsSpawns> mapsSpawns
		{
			get
			{
				return this.GetTable<mapsSpawns>();
			}
		}
		
		public Table<nPcSMasterData> nPcSMasterData
		{
			get
			{
				return this.GetTable<nPcSMasterData>();
			}
		}
		
		public Table<nPcSNames> nPcSNames
		{
			get
			{
				return this.GetTable<nPcSNames>();
			}
		}
		
		public Table<nPcSSpawns> nPcSSpawns
		{
			get
			{
				return this.GetTable<nPcSSpawns>();
			}
		}
	}
	
	#region Start MONO_STRICT
#if MONO_STRICT

	public partial class gWLpR
	{
		
		public gWLpR(IDbConnection connection) : 
				base(connection)
		{
			this.OnCreated();
		}
	}
	#region End MONO_STRICT
	#endregion
#else     // MONO_STRICT
	
	public partial class MySQL
	{
		
		public MySQL(IDbConnection connection) : 
				base(connection, new DbLinq.MySql.MySqlVendor())
		{
			this.OnCreated();
		}
		
		public MySQL(IDbConnection connection, IVendor sqlDialect) : 
				base(connection, sqlDialect)
		{
			this.OnCreated();
		}
		
		public MySQL(IDbConnection connection, MappingSource mappingSource, IVendor sqlDialect) : 
				base(connection, mappingSource, sqlDialect)
		{
			this.OnCreated();
		}
	}
	#region End Not MONO_STRICT
	#endregion
#endif     // MONO_STRICT
	#endregion
	
	[Table(Name="gwlpr.accounts_masterdata")]
	public partial class accountsMasterData : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private int _accountID;
		
		private System.Nullable<int> _balthPtsFree;
		
		private System.Nullable<int> _balthPtsTotal;
		
		private System.Nullable<int> _charID;
		
		private string _email;
		
		private int _groupID;
		
		private byte[] _guiSettings;
		
		private System.Nullable<int> _imperialPtsFree;
		
		private System.Nullable<int> _imperialPtsTotal;
		
		private sbyte _isBanned;
		
		private System.Nullable<int> _kurzickPtsFree;
		
		private System.Nullable<int> _kurzickPtsTotal;
		
		private System.Nullable<int> _luxonPtsFree;
		
		private System.Nullable<int> _luxonPtsTotal;
		
		private string _password;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnaccountIDChanged();
		
		partial void OnaccountIDChanging(int value);
		
		partial void OnbalthPtsFreeChanged();
		
		partial void OnbalthPtsFreeChanging(System.Nullable<int> value);
		
		partial void OnbalthPtsTotalChanged();
		
		partial void OnbalthPtsTotalChanging(System.Nullable<int> value);
		
		partial void OncharIDChanged();
		
		partial void OncharIDChanging(System.Nullable<int> value);
		
		partial void OnemailChanged();
		
		partial void OnemailChanging(string value);
		
		partial void OngroupIDChanged();
		
		partial void OngroupIDChanging(int value);
		
		partial void OnguiSettingsChanged();
		
		partial void OnguiSettingsChanging(byte[] value);
		
		partial void OnimperialPtsFreeChanged();
		
		partial void OnimperialPtsFreeChanging(System.Nullable<int> value);
		
		partial void OnimperialPtsTotalChanged();
		
		partial void OnimperialPtsTotalChanging(System.Nullable<int> value);
		
		partial void OnisBannedChanged();
		
		partial void OnisBannedChanging(sbyte value);
		
		partial void OnkurzickPtsFreeChanged();
		
		partial void OnkurzickPtsFreeChanging(System.Nullable<int> value);
		
		partial void OnkurzickPtsTotalChanged();
		
		partial void OnkurzickPtsTotalChanging(System.Nullable<int> value);
		
		partial void OnluxonPtsFreeChanged();
		
		partial void OnluxonPtsFreeChanging(System.Nullable<int> value);
		
		partial void OnluxonPtsTotalChanged();
		
		partial void OnluxonPtsTotalChanging(System.Nullable<int> value);
		
		partial void OnpasswordChanged();
		
		partial void OnpasswordChanging(string value);
		#endregion
		
		
		public accountsMasterData()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_accountID", Name="AccountID", DbType="int", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int accountID
		{
			get
			{
				return this._accountID;
			}
			set
			{
				if ((_accountID != value))
				{
					this.OnaccountIDChanging(value);
					this.SendPropertyChanging();
					this._accountID = value;
					this.SendPropertyChanged("accountID");
					this.OnaccountIDChanged();
				}
			}
		}
		
		[Column(Storage="_balthPtsFree", Name="BalthPtsFree", DbType="int", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<int> balthPtsFree
		{
			get
			{
				return this._balthPtsFree;
			}
			set
			{
				if ((_balthPtsFree != value))
				{
					this.OnbalthPtsFreeChanging(value);
					this.SendPropertyChanging();
					this._balthPtsFree = value;
					this.SendPropertyChanged("balthPtsFree");
					this.OnbalthPtsFreeChanged();
				}
			}
		}
		
		[Column(Storage="_balthPtsTotal", Name="BalthPtsTotal", DbType="int", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<int> balthPtsTotal
		{
			get
			{
				return this._balthPtsTotal;
			}
			set
			{
				if ((_balthPtsTotal != value))
				{
					this.OnbalthPtsTotalChanging(value);
					this.SendPropertyChanging();
					this._balthPtsTotal = value;
					this.SendPropertyChanged("balthPtsTotal");
					this.OnbalthPtsTotalChanged();
				}
			}
		}
		
		[Column(Storage="_charID", Name="CharID", DbType="int", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<int> charID
		{
			get
			{
				return this._charID;
			}
			set
			{
				if ((_charID != value))
				{
					this.OncharIDChanging(value);
					this.SendPropertyChanging();
					this._charID = value;
					this.SendPropertyChanged("charID");
					this.OncharIDChanged();
				}
			}
		}
		
		[Column(Storage="_email", Name="Email", DbType="char(64)", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public string email
		{
			get
			{
				return this._email;
			}
			set
			{
				if (((_email == value) 
							== false))
				{
					this.OnemailChanging(value);
					this.SendPropertyChanging();
					this._email = value;
					this.SendPropertyChanged("email");
					this.OnemailChanged();
				}
			}
		}
		
		[Column(Storage="_groupID", Name="GroupID", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int groupID
		{
			get
			{
				return this._groupID;
			}
			set
			{
				if ((_groupID != value))
				{
					this.OngroupIDChanging(value);
					this.SendPropertyChanging();
					this._groupID = value;
					this.SendPropertyChanged("groupID");
					this.OngroupIDChanged();
				}
			}
		}
		
		[Column(Storage="_guiSettings", Name="GuiSettings", DbType="blob", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public byte[] guiSettings
		{
			get
			{
				return this._guiSettings;
			}
			set
			{
				if (((_guiSettings == value) 
							== false))
				{
					this.OnguiSettingsChanging(value);
					this.SendPropertyChanging();
					this._guiSettings = value;
					this.SendPropertyChanged("guiSettings");
					this.OnguiSettingsChanged();
				}
			}
		}
		
		[Column(Storage="_imperialPtsFree", Name="ImperialPtsFree", DbType="int", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<int> imperialPtsFree
		{
			get
			{
				return this._imperialPtsFree;
			}
			set
			{
				if ((_imperialPtsFree != value))
				{
					this.OnimperialPtsFreeChanging(value);
					this.SendPropertyChanging();
					this._imperialPtsFree = value;
					this.SendPropertyChanged("imperialPtsFree");
					this.OnimperialPtsFreeChanged();
				}
			}
		}
		
		[Column(Storage="_imperialPtsTotal", Name="ImperialPtsTotal", DbType="int", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<int> imperialPtsTotal
		{
			get
			{
				return this._imperialPtsTotal;
			}
			set
			{
				if ((_imperialPtsTotal != value))
				{
					this.OnimperialPtsTotalChanging(value);
					this.SendPropertyChanging();
					this._imperialPtsTotal = value;
					this.SendPropertyChanged("imperialPtsTotal");
					this.OnimperialPtsTotalChanged();
				}
			}
		}
		
		[Column(Storage="_isBanned", Name="IsBanned", DbType="tinyint(1)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public sbyte isBanned
		{
			get
			{
				return this._isBanned;
			}
			set
			{
				if ((_isBanned != value))
				{
					this.OnisBannedChanging(value);
					this.SendPropertyChanging();
					this._isBanned = value;
					this.SendPropertyChanged("isBanned");
					this.OnisBannedChanged();
				}
			}
		}
		
		[Column(Storage="_kurzickPtsFree", Name="KurzickPtsFree", DbType="int", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<int> kurzickPtsFree
		{
			get
			{
				return this._kurzickPtsFree;
			}
			set
			{
				if ((_kurzickPtsFree != value))
				{
					this.OnkurzickPtsFreeChanging(value);
					this.SendPropertyChanging();
					this._kurzickPtsFree = value;
					this.SendPropertyChanged("kurzickPtsFree");
					this.OnkurzickPtsFreeChanged();
				}
			}
		}
		
		[Column(Storage="_kurzickPtsTotal", Name="KurzickPtsTotal", DbType="int", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<int> kurzickPtsTotal
		{
			get
			{
				return this._kurzickPtsTotal;
			}
			set
			{
				if ((_kurzickPtsTotal != value))
				{
					this.OnkurzickPtsTotalChanging(value);
					this.SendPropertyChanging();
					this._kurzickPtsTotal = value;
					this.SendPropertyChanged("kurzickPtsTotal");
					this.OnkurzickPtsTotalChanged();
				}
			}
		}
		
		[Column(Storage="_luxonPtsFree", Name="LuxonPtsFree", DbType="int", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<int> luxonPtsFree
		{
			get
			{
				return this._luxonPtsFree;
			}
			set
			{
				if ((_luxonPtsFree != value))
				{
					this.OnluxonPtsFreeChanging(value);
					this.SendPropertyChanging();
					this._luxonPtsFree = value;
					this.SendPropertyChanged("luxonPtsFree");
					this.OnluxonPtsFreeChanged();
				}
			}
		}
		
		[Column(Storage="_luxonPtsTotal", Name="LuxonPtsTotal", DbType="int", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<int> luxonPtsTotal
		{
			get
			{
				return this._luxonPtsTotal;
			}
			set
			{
				if ((_luxonPtsTotal != value))
				{
					this.OnluxonPtsTotalChanging(value);
					this.SendPropertyChanging();
					this._luxonPtsTotal = value;
					this.SendPropertyChanged("luxonPtsTotal");
					this.OnluxonPtsTotalChanged();
				}
			}
		}
		
		[Column(Storage="_password", Name="Password", DbType="char(20)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public string password
		{
			get
			{
				return this._password;
			}
			set
			{
				if (((_password == value) 
							== false))
				{
					this.OnpasswordChanging(value);
					this.SendPropertyChanging();
					this._password = value;
					this.SendPropertyChanged("password");
					this.OnpasswordChanged();
				}
			}
		}
		
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			System.ComponentModel.PropertyChangingEventHandler h = this.PropertyChanging;
			if ((h != null))
			{
				h(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler h = this.PropertyChanged;
			if ((h != null))
			{
				h(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[Table(Name="gwlpr.chars_masterdata")]
	public partial class charsMasterData : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private int _accountID;
		
		private sbyte _activeWeaponset;
		
		private int _attrPtsFree;
		
		private int _attrPtsTotal;
		
		private int _charID;
		
		private string _charName;
		
		private int _experiencePts;
		
		private int _inventoryGold;
		
		private sbyte _isPvP;
		
		private long _leadhandWeaponSet1;
		
		private long _leadhandWeaponSet2;
		
		private long _leadhandWeaponSet3;
		
		private long _leadhandWeaponSet4;
		
		private int _level;
		
		private int _lookCampaign;
		
		private int _lookFace;
		
		private int _lookHairColor;
		
		private int _lookHairStyle;
		
		private int _lookHeight;
		
		private int _lookSex;
		
		private int _lookSkinColor;
		
		private int _mapID;
		
		private long _offhandWeaponSet1;
		
		private long _offhandWeaponSet2;
		
		private long _offhandWeaponSet3;
		
		private long _offhandWeaponSet4;
		
		private sbyte _professionPrimary;
		
		private sbyte _professionSecondary;
		
		private sbyte _showHelm;
		
		private byte[] _skillBar;
		
		private int _skillPtsFree;
		
		private int _skillPtsTotal;
		
		private byte[] _skillsAvailable;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnaccountIDChanged();
		
		partial void OnaccountIDChanging(int value);
		
		partial void OnactiveWeaponsetChanged();
		
		partial void OnactiveWeaponsetChanging(sbyte value);
		
		partial void OnattrPtsFreeChanged();
		
		partial void OnattrPtsFreeChanging(int value);
		
		partial void OnattrPtsTotalChanged();
		
		partial void OnattrPtsTotalChanging(int value);
		
		partial void OncharIDChanged();
		
		partial void OncharIDChanging(int value);
		
		partial void OncharNameChanged();
		
		partial void OncharNameChanging(string value);
		
		partial void OnexperiencePtsChanged();
		
		partial void OnexperiencePtsChanging(int value);
		
		partial void OninventoryGoldChanged();
		
		partial void OninventoryGoldChanging(int value);
		
		partial void OnisPvPChanged();
		
		partial void OnisPvPChanging(sbyte value);
		
		partial void OnleadhandWeaponSet1Changed();
		
		partial void OnleadhandWeaponSet1Changing(long value);
		
		partial void OnleadhandWeaponSet2Changed();
		
		partial void OnleadhandWeaponSet2Changing(long value);
		
		partial void OnleadhandWeaponSet3Changed();
		
		partial void OnleadhandWeaponSet3Changing(long value);
		
		partial void OnleadhandWeaponSet4Changed();
		
		partial void OnleadhandWeaponSet4Changing(long value);
		
		partial void OnlevelChanged();
		
		partial void OnlevelChanging(int value);
		
		partial void OnlookCampaignChanged();
		
		partial void OnlookCampaignChanging(int value);
		
		partial void OnlookFaceChanged();
		
		partial void OnlookFaceChanging(int value);
		
		partial void OnlookHairColorChanged();
		
		partial void OnlookHairColorChanging(int value);
		
		partial void OnlookHairStyleChanged();
		
		partial void OnlookHairStyleChanging(int value);
		
		partial void OnlookHeightChanged();
		
		partial void OnlookHeightChanging(int value);
		
		partial void OnlookSexChanged();
		
		partial void OnlookSexChanging(int value);
		
		partial void OnlookSkinColorChanged();
		
		partial void OnlookSkinColorChanging(int value);
		
		partial void OnmapIDChanged();
		
		partial void OnmapIDChanging(int value);
		
		partial void OnoffhandWeaponSet1Changed();
		
		partial void OnoffhandWeaponSet1Changing(long value);
		
		partial void OnoffhandWeaponSet2Changed();
		
		partial void OnoffhandWeaponSet2Changing(long value);
		
		partial void OnoffhandWeaponSet3Changed();
		
		partial void OnoffhandWeaponSet3Changing(long value);
		
		partial void OnoffhandWeaponSet4Changed();
		
		partial void OnoffhandWeaponSet4Changing(long value);
		
		partial void OnprofessionPrimaryChanged();
		
		partial void OnprofessionPrimaryChanging(sbyte value);
		
		partial void OnprofessionSecondaryChanged();
		
		partial void OnprofessionSecondaryChanging(sbyte value);
		
		partial void OnshowHelmChanged();
		
		partial void OnshowHelmChanging(sbyte value);
		
		partial void OnskillBarChanged();
		
		partial void OnskillBarChanging(byte[] value);
		
		partial void OnskillPtsFreeChanged();
		
		partial void OnskillPtsFreeChanging(int value);
		
		partial void OnskillPtsTotalChanged();
		
		partial void OnskillPtsTotalChanging(int value);
		
		partial void OnskillsAvailableChanged();
		
		partial void OnskillsAvailableChanging(byte[] value);
		#endregion
		
		
		public charsMasterData()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_accountID", Name="AccountID", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int accountID
		{
			get
			{
				return this._accountID;
			}
			set
			{
				if ((_accountID != value))
				{
					this.OnaccountIDChanging(value);
					this.SendPropertyChanging();
					this._accountID = value;
					this.SendPropertyChanged("accountID");
					this.OnaccountIDChanged();
				}
			}
		}
		
		[Column(Storage="_activeWeaponset", Name="ActiveWeaponset", DbType="tinyint(1)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public sbyte activeWeaponset
		{
			get
			{
				return this._activeWeaponset;
			}
			set
			{
				if ((_activeWeaponset != value))
				{
					this.OnactiveWeaponsetChanging(value);
					this.SendPropertyChanging();
					this._activeWeaponset = value;
					this.SendPropertyChanged("activeWeaponset");
					this.OnactiveWeaponsetChanged();
				}
			}
		}
		
		[Column(Storage="_attrPtsFree", Name="AttrPtsFree", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int attrPtsFree
		{
			get
			{
				return this._attrPtsFree;
			}
			set
			{
				if ((_attrPtsFree != value))
				{
					this.OnattrPtsFreeChanging(value);
					this.SendPropertyChanging();
					this._attrPtsFree = value;
					this.SendPropertyChanged("attrPtsFree");
					this.OnattrPtsFreeChanged();
				}
			}
		}
		
		[Column(Storage="_attrPtsTotal", Name="AttrPtsTotal", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int attrPtsTotal
		{
			get
			{
				return this._attrPtsTotal;
			}
			set
			{
				if ((_attrPtsTotal != value))
				{
					this.OnattrPtsTotalChanging(value);
					this.SendPropertyChanging();
					this._attrPtsTotal = value;
					this.SendPropertyChanged("attrPtsTotal");
					this.OnattrPtsTotalChanged();
				}
			}
		}
		
		[Column(Storage="_charID", Name="CharID", DbType="int", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int charID
		{
			get
			{
				return this._charID;
			}
			set
			{
				if ((_charID != value))
				{
					this.OncharIDChanging(value);
					this.SendPropertyChanging();
					this._charID = value;
					this.SendPropertyChanged("charID");
					this.OncharIDChanged();
				}
			}
		}
		
		[Column(Storage="_charName", Name="CharName", DbType="char(20)", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public string charName
		{
			get
			{
				return this._charName;
			}
			set
			{
				if (((_charName == value) 
							== false))
				{
					this.OncharNameChanging(value);
					this.SendPropertyChanging();
					this._charName = value;
					this.SendPropertyChanged("charName");
					this.OncharNameChanged();
				}
			}
		}
		
		[Column(Storage="_experiencePts", Name="ExperiencePts", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int experiencePts
		{
			get
			{
				return this._experiencePts;
			}
			set
			{
				if ((_experiencePts != value))
				{
					this.OnexperiencePtsChanging(value);
					this.SendPropertyChanging();
					this._experiencePts = value;
					this.SendPropertyChanged("experiencePts");
					this.OnexperiencePtsChanged();
				}
			}
		}
		
		[Column(Storage="_inventoryGold", Name="InventoryGold", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int inventoryGold
		{
			get
			{
				return this._inventoryGold;
			}
			set
			{
				if ((_inventoryGold != value))
				{
					this.OninventoryGoldChanging(value);
					this.SendPropertyChanging();
					this._inventoryGold = value;
					this.SendPropertyChanged("inventoryGold");
					this.OninventoryGoldChanged();
				}
			}
		}
		
		[Column(Storage="_isPvP", Name="IsPvP", DbType="tinyint(1)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public sbyte isPvP
		{
			get
			{
				return this._isPvP;
			}
			set
			{
				if ((_isPvP != value))
				{
					this.OnisPvPChanging(value);
					this.SendPropertyChanging();
					this._isPvP = value;
					this.SendPropertyChanged("isPvP");
					this.OnisPvPChanged();
				}
			}
		}
		
		[Column(Storage="_leadhandWeaponSet1", Name="LeadhandWeaponSet1", DbType="bigint(20)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public long leadhandWeaponSet1
		{
			get
			{
				return this._leadhandWeaponSet1;
			}
			set
			{
				if ((_leadhandWeaponSet1 != value))
				{
					this.OnleadhandWeaponSet1Changing(value);
					this.SendPropertyChanging();
					this._leadhandWeaponSet1 = value;
					this.SendPropertyChanged("leadhandWeaponSet1");
					this.OnleadhandWeaponSet1Changed();
				}
			}
		}
		
		[Column(Storage="_leadhandWeaponSet2", Name="LeadhandWeaponSet2", DbType="bigint(20)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public long leadhandWeaponSet2
		{
			get
			{
				return this._leadhandWeaponSet2;
			}
			set
			{
				if ((_leadhandWeaponSet2 != value))
				{
					this.OnleadhandWeaponSet2Changing(value);
					this.SendPropertyChanging();
					this._leadhandWeaponSet2 = value;
					this.SendPropertyChanged("leadhandWeaponSet2");
					this.OnleadhandWeaponSet2Changed();
				}
			}
		}
		
		[Column(Storage="_leadhandWeaponSet3", Name="LeadhandWeaponSet3", DbType="bigint(20)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public long leadhandWeaponSet3
		{
			get
			{
				return this._leadhandWeaponSet3;
			}
			set
			{
				if ((_leadhandWeaponSet3 != value))
				{
					this.OnleadhandWeaponSet3Changing(value);
					this.SendPropertyChanging();
					this._leadhandWeaponSet3 = value;
					this.SendPropertyChanged("leadhandWeaponSet3");
					this.OnleadhandWeaponSet3Changed();
				}
			}
		}
		
		[Column(Storage="_leadhandWeaponSet4", Name="LeadhandWeaponSet4", DbType="bigint(20)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public long leadhandWeaponSet4
		{
			get
			{
				return this._leadhandWeaponSet4;
			}
			set
			{
				if ((_leadhandWeaponSet4 != value))
				{
					this.OnleadhandWeaponSet4Changing(value);
					this.SendPropertyChanging();
					this._leadhandWeaponSet4 = value;
					this.SendPropertyChanged("leadhandWeaponSet4");
					this.OnleadhandWeaponSet4Changed();
				}
			}
		}
		
		[Column(Storage="_level", Name="Level", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int level
		{
			get
			{
				return this._level;
			}
			set
			{
				if ((_level != value))
				{
					this.OnlevelChanging(value);
					this.SendPropertyChanging();
					this._level = value;
					this.SendPropertyChanged("level");
					this.OnlevelChanged();
				}
			}
		}
		
		[Column(Storage="_lookCampaign", Name="LookCampaign", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int lookCampaign
		{
			get
			{
				return this._lookCampaign;
			}
			set
			{
				if ((_lookCampaign != value))
				{
					this.OnlookCampaignChanging(value);
					this.SendPropertyChanging();
					this._lookCampaign = value;
					this.SendPropertyChanged("lookCampaign");
					this.OnlookCampaignChanged();
				}
			}
		}
		
		[Column(Storage="_lookFace", Name="LookFace", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int lookFace
		{
			get
			{
				return this._lookFace;
			}
			set
			{
				if ((_lookFace != value))
				{
					this.OnlookFaceChanging(value);
					this.SendPropertyChanging();
					this._lookFace = value;
					this.SendPropertyChanged("lookFace");
					this.OnlookFaceChanged();
				}
			}
		}
		
		[Column(Storage="_lookHairColor", Name="LookHairColor", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int lookHairColor
		{
			get
			{
				return this._lookHairColor;
			}
			set
			{
				if ((_lookHairColor != value))
				{
					this.OnlookHairColorChanging(value);
					this.SendPropertyChanging();
					this._lookHairColor = value;
					this.SendPropertyChanged("lookHairColor");
					this.OnlookHairColorChanged();
				}
			}
		}
		
		[Column(Storage="_lookHairStyle", Name="LookHairStyle", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int lookHairStyle
		{
			get
			{
				return this._lookHairStyle;
			}
			set
			{
				if ((_lookHairStyle != value))
				{
					this.OnlookHairStyleChanging(value);
					this.SendPropertyChanging();
					this._lookHairStyle = value;
					this.SendPropertyChanged("lookHairStyle");
					this.OnlookHairStyleChanged();
				}
			}
		}
		
		[Column(Storage="_lookHeight", Name="LookHeight", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int lookHeight
		{
			get
			{
				return this._lookHeight;
			}
			set
			{
				if ((_lookHeight != value))
				{
					this.OnlookHeightChanging(value);
					this.SendPropertyChanging();
					this._lookHeight = value;
					this.SendPropertyChanged("lookHeight");
					this.OnlookHeightChanged();
				}
			}
		}
		
		[Column(Storage="_lookSex", Name="LookSex", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int lookSex
		{
			get
			{
				return this._lookSex;
			}
			set
			{
				if ((_lookSex != value))
				{
					this.OnlookSexChanging(value);
					this.SendPropertyChanging();
					this._lookSex = value;
					this.SendPropertyChanged("lookSex");
					this.OnlookSexChanged();
				}
			}
		}
		
		[Column(Storage="_lookSkinColor", Name="LookSkinColor", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int lookSkinColor
		{
			get
			{
				return this._lookSkinColor;
			}
			set
			{
				if ((_lookSkinColor != value))
				{
					this.OnlookSkinColorChanging(value);
					this.SendPropertyChanging();
					this._lookSkinColor = value;
					this.SendPropertyChanged("lookSkinColor");
					this.OnlookSkinColorChanged();
				}
			}
		}
		
		[Column(Storage="_mapID", Name="MapID", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int mapID
		{
			get
			{
				return this._mapID;
			}
			set
			{
				if ((_mapID != value))
				{
					this.OnmapIDChanging(value);
					this.SendPropertyChanging();
					this._mapID = value;
					this.SendPropertyChanged("mapID");
					this.OnmapIDChanged();
				}
			}
		}
		
		[Column(Storage="_offhandWeaponSet1", Name="OffhandWeaponSet1", DbType="bigint(20)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public long offhandWeaponSet1
		{
			get
			{
				return this._offhandWeaponSet1;
			}
			set
			{
				if ((_offhandWeaponSet1 != value))
				{
					this.OnoffhandWeaponSet1Changing(value);
					this.SendPropertyChanging();
					this._offhandWeaponSet1 = value;
					this.SendPropertyChanged("offhandWeaponSet1");
					this.OnoffhandWeaponSet1Changed();
				}
			}
		}
		
		[Column(Storage="_offhandWeaponSet2", Name="OffhandWeaponSet2", DbType="bigint(20)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public long offhandWeaponSet2
		{
			get
			{
				return this._offhandWeaponSet2;
			}
			set
			{
				if ((_offhandWeaponSet2 != value))
				{
					this.OnoffhandWeaponSet2Changing(value);
					this.SendPropertyChanging();
					this._offhandWeaponSet2 = value;
					this.SendPropertyChanged("offhandWeaponSet2");
					this.OnoffhandWeaponSet2Changed();
				}
			}
		}
		
		[Column(Storage="_offhandWeaponSet3", Name="OffhandWeaponSet3", DbType="bigint(20)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public long offhandWeaponSet3
		{
			get
			{
				return this._offhandWeaponSet3;
			}
			set
			{
				if ((_offhandWeaponSet3 != value))
				{
					this.OnoffhandWeaponSet3Changing(value);
					this.SendPropertyChanging();
					this._offhandWeaponSet3 = value;
					this.SendPropertyChanged("offhandWeaponSet3");
					this.OnoffhandWeaponSet3Changed();
				}
			}
		}
		
		[Column(Storage="_offhandWeaponSet4", Name="OffhandWeaponSet4", DbType="bigint(20)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public long offhandWeaponSet4
		{
			get
			{
				return this._offhandWeaponSet4;
			}
			set
			{
				if ((_offhandWeaponSet4 != value))
				{
					this.OnoffhandWeaponSet4Changing(value);
					this.SendPropertyChanging();
					this._offhandWeaponSet4 = value;
					this.SendPropertyChanged("offhandWeaponSet4");
					this.OnoffhandWeaponSet4Changed();
				}
			}
		}
		
		[Column(Storage="_professionPrimary", Name="ProfessionPrimary", DbType="tinyint(4)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public sbyte professionPrimary
		{
			get
			{
				return this._professionPrimary;
			}
			set
			{
				if ((_professionPrimary != value))
				{
					this.OnprofessionPrimaryChanging(value);
					this.SendPropertyChanging();
					this._professionPrimary = value;
					this.SendPropertyChanged("professionPrimary");
					this.OnprofessionPrimaryChanged();
				}
			}
		}
		
		[Column(Storage="_professionSecondary", Name="ProfessionSecondary", DbType="tinyint(4)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public sbyte professionSecondary
		{
			get
			{
				return this._professionSecondary;
			}
			set
			{
				if ((_professionSecondary != value))
				{
					this.OnprofessionSecondaryChanging(value);
					this.SendPropertyChanging();
					this._professionSecondary = value;
					this.SendPropertyChanged("professionSecondary");
					this.OnprofessionSecondaryChanged();
				}
			}
		}
		
		[Column(Storage="_showHelm", Name="ShowHelm", DbType="tinyint(1)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public sbyte showHelm
		{
			get
			{
				return this._showHelm;
			}
			set
			{
				if ((_showHelm != value))
				{
					this.OnshowHelmChanging(value);
					this.SendPropertyChanging();
					this._showHelm = value;
					this.SendPropertyChanged("showHelm");
					this.OnshowHelmChanged();
				}
			}
		}
		
		[Column(Storage="_skillBar", Name="SkillBar", DbType="blob", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public byte[] skillBar
		{
			get
			{
				return this._skillBar;
			}
			set
			{
				if (((_skillBar == value) 
							== false))
				{
					this.OnskillBarChanging(value);
					this.SendPropertyChanging();
					this._skillBar = value;
					this.SendPropertyChanged("skillBar");
					this.OnskillBarChanged();
				}
			}
		}
		
		[Column(Storage="_skillPtsFree", Name="SkillPtsFree", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int skillPtsFree
		{
			get
			{
				return this._skillPtsFree;
			}
			set
			{
				if ((_skillPtsFree != value))
				{
					this.OnskillPtsFreeChanging(value);
					this.SendPropertyChanging();
					this._skillPtsFree = value;
					this.SendPropertyChanged("skillPtsFree");
					this.OnskillPtsFreeChanged();
				}
			}
		}
		
		[Column(Storage="_skillPtsTotal", Name="SkillPtsTotal", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int skillPtsTotal
		{
			get
			{
				return this._skillPtsTotal;
			}
			set
			{
				if ((_skillPtsTotal != value))
				{
					this.OnskillPtsTotalChanging(value);
					this.SendPropertyChanging();
					this._skillPtsTotal = value;
					this.SendPropertyChanged("skillPtsTotal");
					this.OnskillPtsTotalChanged();
				}
			}
		}
		
		[Column(Storage="_skillsAvailable", Name="SkillsAvailable", DbType="blob", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public byte[] skillsAvailable
		{
			get
			{
				return this._skillsAvailable;
			}
			set
			{
				if (((_skillsAvailable == value) 
							== false))
				{
					this.OnskillsAvailableChanging(value);
					this.SendPropertyChanging();
					this._skillsAvailable = value;
					this.SendPropertyChanged("skillsAvailable");
					this.OnskillsAvailableChanged();
				}
			}
		}
		
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			System.ComponentModel.PropertyChangingEventHandler h = this.PropertyChanging;
			if ((h != null))
			{
				h(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler h = this.PropertyChanged;
			if ((h != null))
			{
				h(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[Table(Name="gwlpr.groups_commands")]
	public partial class groupsCommands : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private int _commandID;
		
		private string _commandName;
		
		private int _groupID;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OncommandIDChanged();
		
		partial void OncommandIDChanging(int value);
		
		partial void OncommandNameChanged();
		
		partial void OncommandNameChanging(string value);
		
		partial void OngroupIDChanged();
		
		partial void OngroupIDChanging(int value);
		#endregion
		
		
		public groupsCommands()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_commandID", Name="CommandID", DbType="int", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int commandID
		{
			get
			{
				return this._commandID;
			}
			set
			{
				if ((_commandID != value))
				{
					this.OncommandIDChanging(value);
					this.SendPropertyChanging();
					this._commandID = value;
					this.SendPropertyChanged("commandID");
					this.OncommandIDChanged();
				}
			}
		}
		
		[Column(Storage="_commandName", Name="CommandName", DbType="char(16)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public string commandName
		{
			get
			{
				return this._commandName;
			}
			set
			{
				if (((_commandName == value) 
							== false))
				{
					this.OncommandNameChanging(value);
					this.SendPropertyChanging();
					this._commandName = value;
					this.SendPropertyChanged("commandName");
					this.OncommandNameChanged();
				}
			}
		}
		
		[Column(Storage="_groupID", Name="GroupID", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int groupID
		{
			get
			{
				return this._groupID;
			}
			set
			{
				if ((_groupID != value))
				{
					this.OngroupIDChanging(value);
					this.SendPropertyChanging();
					this._groupID = value;
					this.SendPropertyChanged("groupID");
					this.OngroupIDChanged();
				}
			}
		}
		
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			System.ComponentModel.PropertyChangingEventHandler h = this.PropertyChanging;
			if ((h != null))
			{
				h(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler h = this.PropertyChanged;
			if ((h != null))
			{
				h(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[Table(Name="gwlpr.groups_masterdata")]
	public partial class groupsMasterData : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private int _groupChatColor;
		
		private int _groupID;
		
		private string _groupName;
		
		private string _groupPrefix;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OngroupChatColorChanged();
		
		partial void OngroupChatColorChanging(int value);
		
		partial void OngroupIDChanged();
		
		partial void OngroupIDChanging(int value);
		
		partial void OngroupNameChanged();
		
		partial void OngroupNameChanging(string value);
		
		partial void OngroupPrefixChanged();
		
		partial void OngroupPrefixChanging(string value);
		#endregion
		
		
		public groupsMasterData()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_groupChatColor", Name="GroupChatColor", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int groupChatColor
		{
			get
			{
				return this._groupChatColor;
			}
			set
			{
				if ((_groupChatColor != value))
				{
					this.OngroupChatColorChanging(value);
					this.SendPropertyChanging();
					this._groupChatColor = value;
					this.SendPropertyChanged("groupChatColor");
					this.OngroupChatColorChanged();
				}
			}
		}
		
		[Column(Storage="_groupID", Name="GroupID", DbType="int", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int groupID
		{
			get
			{
				return this._groupID;
			}
			set
			{
				if ((_groupID != value))
				{
					this.OngroupIDChanging(value);
					this.SendPropertyChanging();
					this._groupID = value;
					this.SendPropertyChanged("groupID");
					this.OngroupIDChanged();
				}
			}
		}
		
		[Column(Storage="_groupName", Name="GroupName", DbType="char(64)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public string groupName
		{
			get
			{
				return this._groupName;
			}
			set
			{
				if (((_groupName == value) 
							== false))
				{
					this.OngroupNameChanging(value);
					this.SendPropertyChanging();
					this._groupName = value;
					this.SendPropertyChanged("groupName");
					this.OngroupNameChanged();
				}
			}
		}
		
		[Column(Storage="_groupPrefix", Name="GroupPrefix", DbType="char(8)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public string groupPrefix
		{
			get
			{
				return this._groupPrefix;
			}
			set
			{
				if (((_groupPrefix == value) 
							== false))
				{
					this.OngroupPrefixChanging(value);
					this.SendPropertyChanging();
					this._groupPrefix = value;
					this.SendPropertyChanged("groupPrefix");
					this.OngroupPrefixChanged();
				}
			}
		}
		
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			System.ComponentModel.PropertyChangingEventHandler h = this.PropertyChanging;
			if ((h != null))
			{
				h(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler h = this.PropertyChanged;
			if ((h != null))
			{
				h(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[Table(Name="gwlpr.items_masterdata")]
	public partial class itemsMasterData : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private long _gameItemFileID;
		
		private int _gameItemID;
		
		private int _itemID;
		
		private int _itemType;
		
		private string _name;
		
		private sbyte _profession;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OngameItemFileIDChanged();
		
		partial void OngameItemFileIDChanging(long value);
		
		partial void OngameItemIDChanged();
		
		partial void OngameItemIDChanging(int value);
		
		partial void OnitemIDChanged();
		
		partial void OnitemIDChanging(int value);
		
		partial void OnitemTypeChanged();
		
		partial void OnitemTypeChanging(int value);
		
		partial void OnnameChanged();
		
		partial void OnnameChanging(string value);
		
		partial void OnprofessionChanged();
		
		partial void OnprofessionChanging(sbyte value);
		#endregion
		
		
		public itemsMasterData()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_gameItemFileID", Name="GameItemFileID", DbType="bigint", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public long gameItemFileID
		{
			get
			{
				return this._gameItemFileID;
			}
			set
			{
				if ((_gameItemFileID != value))
				{
					this.OngameItemFileIDChanging(value);
					this.SendPropertyChanging();
					this._gameItemFileID = value;
					this.SendPropertyChanged("gameItemFileID");
					this.OngameItemFileIDChanged();
				}
			}
		}
		
		[Column(Storage="_gameItemID", Name="GameItemID", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int gameItemID
		{
			get
			{
				return this._gameItemID;
			}
			set
			{
				if ((_gameItemID != value))
				{
					this.OngameItemIDChanging(value);
					this.SendPropertyChanging();
					this._gameItemID = value;
					this.SendPropertyChanged("gameItemID");
					this.OngameItemIDChanged();
				}
			}
		}
		
		[Column(Storage="_itemID", Name="ItemID", DbType="int", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int itemID
		{
			get
			{
				return this._itemID;
			}
			set
			{
				if ((_itemID != value))
				{
					this.OnitemIDChanging(value);
					this.SendPropertyChanging();
					this._itemID = value;
					this.SendPropertyChanged("itemID");
					this.OnitemIDChanged();
				}
			}
		}
		
		[Column(Storage="_itemType", Name="ItemType", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int itemType
		{
			get
			{
				return this._itemType;
			}
			set
			{
				if ((_itemType != value))
				{
					this.OnitemTypeChanging(value);
					this.SendPropertyChanging();
					this._itemType = value;
					this.SendPropertyChanged("itemType");
					this.OnitemTypeChanged();
				}
			}
		}
		
		[Column(Storage="_name", Name="Name", DbType="char(28)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public string name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (((_name == value) 
							== false))
				{
					this.OnnameChanging(value);
					this.SendPropertyChanging();
					this._name = value;
					this.SendPropertyChanged("name");
					this.OnnameChanged();
				}
			}
		}
		
		[Column(Storage="_profession", Name="Profession", DbType="tinyint(8)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public sbyte profession
		{
			get
			{
				return this._profession;
			}
			set
			{
				if ((_profession != value))
				{
					this.OnprofessionChanging(value);
					this.SendPropertyChanging();
					this._profession = value;
					this.SendPropertyChanged("profession");
					this.OnprofessionChanged();
				}
			}
		}
		
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			System.ComponentModel.PropertyChangingEventHandler h = this.PropertyChanging;
			if ((h != null))
			{
				h(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler h = this.PropertyChanged;
			if ((h != null))
			{
				h(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[Table(Name="gwlpr.items_personaldata")]
	public partial class itemsPerSonALData : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private int _accountID;
		
		private int _charID;
		
		private System.Nullable<int> _creatorCharID;
		
		private string _creatorName;
		
		private int _dyeColor;
		
		private int _flags;
		
		private int _itemID;
		
		private long _personalItemID;
		
		private int _quantity;
		
		private int _slot;
		
		private byte[] _stats;
		
		private int _storage;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnaccountIDChanged();
		
		partial void OnaccountIDChanging(int value);
		
		partial void OncharIDChanged();
		
		partial void OncharIDChanging(int value);
		
		partial void OncreatorCharIDChanged();
		
		partial void OncreatorCharIDChanging(System.Nullable<int> value);
		
		partial void OncreatorNameChanged();
		
		partial void OncreatorNameChanging(string value);
		
		partial void OndyeColorChanged();
		
		partial void OndyeColorChanging(int value);
		
		partial void OnflagsChanged();
		
		partial void OnflagsChanging(int value);
		
		partial void OnitemIDChanged();
		
		partial void OnitemIDChanging(int value);
		
		partial void OnpersonalItemIDChanged();
		
		partial void OnpersonalItemIDChanging(long value);
		
		partial void OnquantityChanged();
		
		partial void OnquantityChanging(int value);
		
		partial void OnslotChanged();
		
		partial void OnslotChanging(int value);
		
		partial void OnstatsChanged();
		
		partial void OnstatsChanging(byte[] value);
		
		partial void OnstorageChanged();
		
		partial void OnstorageChanging(int value);
		#endregion
		
		
		public itemsPerSonALData()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_accountID", Name="AccountID", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int accountID
		{
			get
			{
				return this._accountID;
			}
			set
			{
				if ((_accountID != value))
				{
					this.OnaccountIDChanging(value);
					this.SendPropertyChanging();
					this._accountID = value;
					this.SendPropertyChanged("accountID");
					this.OnaccountIDChanged();
				}
			}
		}
		
		[Column(Storage="_charID", Name="CharID", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int charID
		{
			get
			{
				return this._charID;
			}
			set
			{
				if ((_charID != value))
				{
					this.OncharIDChanging(value);
					this.SendPropertyChanging();
					this._charID = value;
					this.SendPropertyChanged("charID");
					this.OncharIDChanged();
				}
			}
		}
		
		[Column(Storage="_creatorCharID", Name="CreatorCharID", DbType="int", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<int> creatorCharID
		{
			get
			{
				return this._creatorCharID;
			}
			set
			{
				if ((_creatorCharID != value))
				{
					this.OncreatorCharIDChanging(value);
					this.SendPropertyChanging();
					this._creatorCharID = value;
					this.SendPropertyChanged("creatorCharID");
					this.OncreatorCharIDChanged();
				}
			}
		}
		
		[Column(Storage="_creatorName", Name="CreatorName", DbType="char(20)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string creatorName
		{
			get
			{
				return this._creatorName;
			}
			set
			{
				if (((_creatorName == value) 
							== false))
				{
					this.OncreatorNameChanging(value);
					this.SendPropertyChanging();
					this._creatorName = value;
					this.SendPropertyChanged("creatorName");
					this.OncreatorNameChanged();
				}
			}
		}
		
		[Column(Storage="_dyeColor", Name="DyeColor", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int dyeColor
		{
			get
			{
				return this._dyeColor;
			}
			set
			{
				if ((_dyeColor != value))
				{
					this.OndyeColorChanging(value);
					this.SendPropertyChanging();
					this._dyeColor = value;
					this.SendPropertyChanged("dyeColor");
					this.OndyeColorChanged();
				}
			}
		}
		
		[Column(Storage="_flags", Name="Flags", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int flags
		{
			get
			{
				return this._flags;
			}
			set
			{
				if ((_flags != value))
				{
					this.OnflagsChanging(value);
					this.SendPropertyChanging();
					this._flags = value;
					this.SendPropertyChanged("flags");
					this.OnflagsChanged();
				}
			}
		}
		
		[Column(Storage="_itemID", Name="ItemID", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int itemID
		{
			get
			{
				return this._itemID;
			}
			set
			{
				if ((_itemID != value))
				{
					this.OnitemIDChanging(value);
					this.SendPropertyChanging();
					this._itemID = value;
					this.SendPropertyChanged("itemID");
					this.OnitemIDChanged();
				}
			}
		}
		
		[Column(Storage="_personalItemID", Name="PersonalItemID", DbType="bigint(20)", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public long personalItemID
		{
			get
			{
				return this._personalItemID;
			}
			set
			{
				if ((_personalItemID != value))
				{
					this.OnpersonalItemIDChanging(value);
					this.SendPropertyChanging();
					this._personalItemID = value;
					this.SendPropertyChanged("personalItemID");
					this.OnpersonalItemIDChanged();
				}
			}
		}
		
		[Column(Storage="_quantity", Name="Quantity", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int quantity
		{
			get
			{
				return this._quantity;
			}
			set
			{
				if ((_quantity != value))
				{
					this.OnquantityChanging(value);
					this.SendPropertyChanging();
					this._quantity = value;
					this.SendPropertyChanged("quantity");
					this.OnquantityChanged();
				}
			}
		}
		
		[Column(Storage="_slot", Name="Slot", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int slot
		{
			get
			{
				return this._slot;
			}
			set
			{
				if ((_slot != value))
				{
					this.OnslotChanging(value);
					this.SendPropertyChanging();
					this._slot = value;
					this.SendPropertyChanged("slot");
					this.OnslotChanged();
				}
			}
		}
		
		[Column(Storage="_stats", Name="Stats", DbType="blob", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public byte[] stats
		{
			get
			{
				return this._stats;
			}
			set
			{
				if (((_stats == value) 
							== false))
				{
					this.OnstatsChanging(value);
					this.SendPropertyChanging();
					this._stats = value;
					this.SendPropertyChanged("stats");
					this.OnstatsChanged();
				}
			}
		}
		
		[Column(Storage="_storage", Name="Storage", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int storage
		{
			get
			{
				return this._storage;
			}
			set
			{
				if ((_storage != value))
				{
					this.OnstorageChanging(value);
					this.SendPropertyChanging();
					this._storage = value;
					this.SendPropertyChanged("storage");
					this.OnstorageChanged();
				}
			}
		}
		
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			System.ComponentModel.PropertyChangingEventHandler h = this.PropertyChanging;
			if ((h != null))
			{
				h(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler h = this.PropertyChanged;
			if ((h != null))
			{
				h(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[Table(Name="gwlpr.items_predefineddata")]
	public partial class itemsPredefinedData : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private int _dyeColor;
		
		private int _flags;
		
		private int _itemID;
		
		private long _preDefinedItemID;
		
		private byte[] _stats;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OndyeColorChanged();
		
		partial void OndyeColorChanging(int value);
		
		partial void OnflagsChanged();
		
		partial void OnflagsChanging(int value);
		
		partial void OnitemIDChanged();
		
		partial void OnitemIDChanging(int value);
		
		partial void OnpreDefinedItemIDChanged();
		
		partial void OnpreDefinedItemIDChanging(long value);
		
		partial void OnstatsChanged();
		
		partial void OnstatsChanging(byte[] value);
		#endregion
		
		
		public itemsPredefinedData()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_dyeColor", Name="DyeColor", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int dyeColor
		{
			get
			{
				return this._dyeColor;
			}
			set
			{
				if ((_dyeColor != value))
				{
					this.OndyeColorChanging(value);
					this.SendPropertyChanging();
					this._dyeColor = value;
					this.SendPropertyChanged("dyeColor");
					this.OndyeColorChanged();
				}
			}
		}
		
		[Column(Storage="_flags", Name="Flags", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int flags
		{
			get
			{
				return this._flags;
			}
			set
			{
				if ((_flags != value))
				{
					this.OnflagsChanging(value);
					this.SendPropertyChanging();
					this._flags = value;
					this.SendPropertyChanged("flags");
					this.OnflagsChanged();
				}
			}
		}
		
		[Column(Storage="_itemID", Name="ItemID", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int itemID
		{
			get
			{
				return this._itemID;
			}
			set
			{
				if ((_itemID != value))
				{
					this.OnitemIDChanging(value);
					this.SendPropertyChanging();
					this._itemID = value;
					this.SendPropertyChanged("itemID");
					this.OnitemIDChanged();
				}
			}
		}
		
		[Column(Storage="_preDefinedItemID", Name="PreDefinedItemID", DbType="bigint(20)", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public long preDefinedItemID
		{
			get
			{
				return this._preDefinedItemID;
			}
			set
			{
				if ((_preDefinedItemID != value))
				{
					this.OnpreDefinedItemIDChanging(value);
					this.SendPropertyChanging();
					this._preDefinedItemID = value;
					this.SendPropertyChanged("preDefinedItemID");
					this.OnpreDefinedItemIDChanged();
				}
			}
		}
		
		[Column(Storage="_stats", Name="Stats", DbType="blob", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public byte[] stats
		{
			get
			{
				return this._stats;
			}
			set
			{
				if (((_stats == value) 
							== false))
				{
					this.OnstatsChanging(value);
					this.SendPropertyChanging();
					this._stats = value;
					this.SendPropertyChanged("stats");
					this.OnstatsChanged();
				}
			}
		}
		
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			System.ComponentModel.PropertyChangingEventHandler h = this.PropertyChanging;
			if ((h != null))
			{
				h(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler h = this.PropertyChanged;
			if ((h != null))
			{
				h(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[Table(Name="gwlpr.maps_masterdata")]
	public partial class mapsMasterData : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private int _gameMapFileID;
		
		private int _gameMapID;
		
		private string _gameMapName;
		
		private int _mapID;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OngameMapFileIDChanged();
		
		partial void OngameMapFileIDChanging(int value);
		
		partial void OngameMapIDChanged();
		
		partial void OngameMapIDChanging(int value);
		
		partial void OngameMapNameChanged();
		
		partial void OngameMapNameChanging(string value);
		
		partial void OnmapIDChanged();
		
		partial void OnmapIDChanging(int value);
		#endregion
		
		
		public mapsMasterData()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_gameMapFileID", Name="GameMapFileID", DbType="int", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int gameMapFileID
		{
			get
			{
				return this._gameMapFileID;
			}
			set
			{
				if ((_gameMapFileID != value))
				{
					this.OngameMapFileIDChanging(value);
					this.SendPropertyChanging();
					this._gameMapFileID = value;
					this.SendPropertyChanged("gameMapFileID");
					this.OngameMapFileIDChanged();
				}
			}
		}
		
		[Column(Storage="_gameMapID", Name="GameMapID", DbType="int", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int gameMapID
		{
			get
			{
				return this._gameMapID;
			}
			set
			{
				if ((_gameMapID != value))
				{
					this.OngameMapIDChanging(value);
					this.SendPropertyChanging();
					this._gameMapID = value;
					this.SendPropertyChanged("gameMapID");
					this.OngameMapIDChanged();
				}
			}
		}
		
		[Column(Storage="_gameMapName", Name="GameMapName", DbType="char(64)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public string gameMapName
		{
			get
			{
				return this._gameMapName;
			}
			set
			{
				if (((_gameMapName == value) 
							== false))
				{
					this.OngameMapNameChanging(value);
					this.SendPropertyChanging();
					this._gameMapName = value;
					this.SendPropertyChanged("gameMapName");
					this.OngameMapNameChanged();
				}
			}
		}
		
		[Column(Storage="_mapID", Name="MapID", DbType="int", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int mapID
		{
			get
			{
				return this._mapID;
			}
			set
			{
				if ((_mapID != value))
				{
					this.OnmapIDChanging(value);
					this.SendPropertyChanging();
					this._mapID = value;
					this.SendPropertyChanged("mapID");
					this.OnmapIDChanged();
				}
			}
		}
		
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			System.ComponentModel.PropertyChangingEventHandler h = this.PropertyChanging;
			if ((h != null))
			{
				h(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler h = this.PropertyChanged;
			if ((h != null))
			{
				h(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[Table(Name="gwlpr.maps_spawns")]
	public partial class mapsSpawns : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private sbyte _isOutpost;
		
		private sbyte _isPvE;
		
		private int _mapID;
		
		private int _spawnID;
		
		private int _spawnPlane;
		
		private float _spawnRadius;
		
		private float _spawnX;
		
		private float _spawnY;
		
		private System.Nullable<int> _teamSpawnNumber;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnisOutpostChanged();
		
		partial void OnisOutpostChanging(sbyte value);
		
		partial void OnisPvEChanged();
		
		partial void OnisPvEChanging(sbyte value);
		
		partial void OnmapIDChanged();
		
		partial void OnmapIDChanging(int value);
		
		partial void OnspawnIDChanged();
		
		partial void OnspawnIDChanging(int value);
		
		partial void OnspawnPlaneChanged();
		
		partial void OnspawnPlaneChanging(int value);
		
		partial void OnspawnRadiusChanged();
		
		partial void OnspawnRadiusChanging(float value);
		
		partial void OnspawnXChanged();
		
		partial void OnspawnXChanging(float value);
		
		partial void OnspawnYChanged();
		
		partial void OnspawnYChanging(float value);
		
		partial void OnteamSpawnNumberChanged();
		
		partial void OnteamSpawnNumberChanging(System.Nullable<int> value);
		#endregion
		
		
		public mapsSpawns()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_isOutpost", Name="IsOutpost", DbType="tinyint(1)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public sbyte isOutpost
		{
			get
			{
				return this._isOutpost;
			}
			set
			{
				if ((_isOutpost != value))
				{
					this.OnisOutpostChanging(value);
					this.SendPropertyChanging();
					this._isOutpost = value;
					this.SendPropertyChanged("isOutpost");
					this.OnisOutpostChanged();
				}
			}
		}
		
		[Column(Storage="_isPvE", Name="IsPvE", DbType="tinyint(1)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public sbyte isPvE
		{
			get
			{
				return this._isPvE;
			}
			set
			{
				if ((_isPvE != value))
				{
					this.OnisPvEChanging(value);
					this.SendPropertyChanging();
					this._isPvE = value;
					this.SendPropertyChanged("isPvE");
					this.OnisPvEChanged();
				}
			}
		}
		
		[Column(Storage="_mapID", Name="MapID", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int mapID
		{
			get
			{
				return this._mapID;
			}
			set
			{
				if ((_mapID != value))
				{
					this.OnmapIDChanging(value);
					this.SendPropertyChanging();
					this._mapID = value;
					this.SendPropertyChanged("mapID");
					this.OnmapIDChanged();
				}
			}
		}
		
		[Column(Storage="_spawnID", Name="SpawnID", DbType="int", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int spawnID
		{
			get
			{
				return this._spawnID;
			}
			set
			{
				if ((_spawnID != value))
				{
					this.OnspawnIDChanging(value);
					this.SendPropertyChanging();
					this._spawnID = value;
					this.SendPropertyChanged("spawnID");
					this.OnspawnIDChanged();
				}
			}
		}
		
		[Column(Storage="_spawnPlane", Name="SpawnPlane", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int spawnPlane
		{
			get
			{
				return this._spawnPlane;
			}
			set
			{
				if ((_spawnPlane != value))
				{
					this.OnspawnPlaneChanging(value);
					this.SendPropertyChanging();
					this._spawnPlane = value;
					this.SendPropertyChanged("spawnPlane");
					this.OnspawnPlaneChanged();
				}
			}
		}
		
		[Column(Storage="_spawnRadius", Name="SpawnRadius", DbType="float", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public float spawnRadius
		{
			get
			{
				return this._spawnRadius;
			}
			set
			{
				if ((_spawnRadius != value))
				{
					this.OnspawnRadiusChanging(value);
					this.SendPropertyChanging();
					this._spawnRadius = value;
					this.SendPropertyChanged("spawnRadius");
					this.OnspawnRadiusChanged();
				}
			}
		}
		
		[Column(Storage="_spawnX", Name="SpawnX", DbType="float", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public float spawnX
		{
			get
			{
				return this._spawnX;
			}
			set
			{
				if ((_spawnX != value))
				{
					this.OnspawnXChanging(value);
					this.SendPropertyChanging();
					this._spawnX = value;
					this.SendPropertyChanged("spawnX");
					this.OnspawnXChanged();
				}
			}
		}
		
		[Column(Storage="_spawnY", Name="SpawnY", DbType="float", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public float spawnY
		{
			get
			{
				return this._spawnY;
			}
			set
			{
				if ((_spawnY != value))
				{
					this.OnspawnYChanging(value);
					this.SendPropertyChanging();
					this._spawnY = value;
					this.SendPropertyChanged("spawnY");
					this.OnspawnYChanged();
				}
			}
		}
		
		[Column(Storage="_teamSpawnNumber", Name="TeamSpawnNumber", DbType="int", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public System.Nullable<int> teamSpawnNumber
		{
			get
			{
				return this._teamSpawnNumber;
			}
			set
			{
				if ((_teamSpawnNumber != value))
				{
					this.OnteamSpawnNumberChanging(value);
					this.SendPropertyChanging();
					this._teamSpawnNumber = value;
					this.SendPropertyChanged("teamSpawnNumber");
					this.OnteamSpawnNumberChanged();
				}
			}
		}
		
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			System.ComponentModel.PropertyChangingEventHandler h = this.PropertyChanging;
			if ((h != null))
			{
				h(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler h = this.PropertyChanged;
			if ((h != null))
			{
				h(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[Table(Name="gwlpr.npcs_masterdata")]
	public partial class nPcSMasterData : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private byte[] _appearance;
		
		private byte[] _modelHash;
		
		private int _npcFileID;
		
		private int _npcID;
		
		private string _npcName;
		
		private int _professionFlags;
		
		private int _scale;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnappearanceChanged();
		
		partial void OnappearanceChanging(byte[] value);
		
		partial void OnmodelHashChanged();
		
		partial void OnmodelHashChanging(byte[] value);
		
		partial void OnnpcFileIDChanged();
		
		partial void OnnpcFileIDChanging(int value);
		
		partial void OnnpcIDChanged();
		
		partial void OnnpcIDChanging(int value);
		
		partial void OnnpcNameChanged();
		
		partial void OnnpcNameChanging(string value);
		
		partial void OnprofessionFlagsChanged();
		
		partial void OnprofessionFlagsChanging(int value);
		
		partial void OnscaleChanged();
		
		partial void OnscaleChanging(int value);
		#endregion
		
		
		public nPcSMasterData()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_appearance", Name="Appearance", DbType="blob", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public byte[] appearance
		{
			get
			{
				return this._appearance;
			}
			set
			{
				if (((_appearance == value) 
							== false))
				{
					this.OnappearanceChanging(value);
					this.SendPropertyChanging();
					this._appearance = value;
					this.SendPropertyChanged("appearance");
					this.OnappearanceChanged();
				}
			}
		}
		
		[Column(Storage="_modelHash", Name="ModelHash", DbType="blob", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public byte[] modelHash
		{
			get
			{
				return this._modelHash;
			}
			set
			{
				if (((_modelHash == value) 
							== false))
				{
					this.OnmodelHashChanging(value);
					this.SendPropertyChanging();
					this._modelHash = value;
					this.SendPropertyChanged("modelHash");
					this.OnmodelHashChanged();
				}
			}
		}
		
		[Column(Storage="_npcFileID", Name="NpcFileID", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int npcFileID
		{
			get
			{
				return this._npcFileID;
			}
			set
			{
				if ((_npcFileID != value))
				{
					this.OnnpcFileIDChanging(value);
					this.SendPropertyChanging();
					this._npcFileID = value;
					this.SendPropertyChanged("npcFileID");
					this.OnnpcFileIDChanged();
				}
			}
		}
		
		[Column(Storage="_npcID", Name="NpcID", DbType="int", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int npcID
		{
			get
			{
				return this._npcID;
			}
			set
			{
				if ((_npcID != value))
				{
					this.OnnpcIDChanging(value);
					this.SendPropertyChanging();
					this._npcID = value;
					this.SendPropertyChanged("npcID");
					this.OnnpcIDChanged();
				}
			}
		}
		
		[Column(Storage="_npcName", Name="NpcName", DbType="char(128)", AutoSync=AutoSync.Never)]
		[DebuggerNonUserCode()]
		public string npcName
		{
			get
			{
				return this._npcName;
			}
			set
			{
				if (((_npcName == value) 
							== false))
				{
					this.OnnpcNameChanging(value);
					this.SendPropertyChanging();
					this._npcName = value;
					this.SendPropertyChanged("npcName");
					this.OnnpcNameChanged();
				}
			}
		}
		
		[Column(Storage="_professionFlags", Name="ProfessionFlags", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int professionFlags
		{
			get
			{
				return this._professionFlags;
			}
			set
			{
				if ((_professionFlags != value))
				{
					this.OnprofessionFlagsChanging(value);
					this.SendPropertyChanging();
					this._professionFlags = value;
					this.SendPropertyChanged("professionFlags");
					this.OnprofessionFlagsChanged();
				}
			}
		}
		
		[Column(Storage="_scale", Name="Scale", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int scale
		{
			get
			{
				return this._scale;
			}
			set
			{
				if ((_scale != value))
				{
					this.OnscaleChanging(value);
					this.SendPropertyChanging();
					this._scale = value;
					this.SendPropertyChanged("scale");
					this.OnscaleChanged();
				}
			}
		}
		
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			System.ComponentModel.PropertyChangingEventHandler h = this.PropertyChanging;
			if ((h != null))
			{
				h(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler h = this.PropertyChanged;
			if ((h != null))
			{
				h(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[Table(Name="gwlpr.npcs_names")]
	public partial class nPcSNames : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private string _name;
		
		private byte[] _nameHash;
		
		private int _nameID;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnnameChanged();
		
		partial void OnnameChanging(string value);
		
		partial void OnnameHashChanged();
		
		partial void OnnameHashChanging(byte[] value);
		
		partial void OnnameIDChanged();
		
		partial void OnnameIDChanging(int value);
		#endregion
		
		
		public nPcSNames()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_name", Name="Name", DbType="char(128)", IsPrimaryKey=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public string name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (((_name == value) 
							== false))
				{
					this.OnnameChanging(value);
					this.SendPropertyChanging();
					this._name = value;
					this.SendPropertyChanged("name");
					this.OnnameChanged();
				}
			}
		}
		
		[Column(Storage="_nameHash", Name="NameHash", DbType="blob", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public byte[] nameHash
		{
			get
			{
				return this._nameHash;
			}
			set
			{
				if (((_nameHash == value) 
							== false))
				{
					this.OnnameHashChanging(value);
					this.SendPropertyChanging();
					this._nameHash = value;
					this.SendPropertyChanged("nameHash");
					this.OnnameHashChanged();
				}
			}
		}
		
		[Column(Storage="_nameID", Name="NameID", DbType="int", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int nameID
		{
			get
			{
				return this._nameID;
			}
			set
			{
				if ((_nameID != value))
				{
					this.OnnameIDChanging(value);
					this.SendPropertyChanging();
					this._nameID = value;
					this.SendPropertyChanged("nameID");
					this.OnnameIDChanged();
				}
			}
		}
		
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			System.ComponentModel.PropertyChangingEventHandler h = this.PropertyChanging;
			if ((h != null))
			{
				h(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler h = this.PropertyChanged;
			if ((h != null))
			{
				h(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[Table(Name="gwlpr.npcs_spawns")]
	public partial class nPcSSpawns : System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.INotifyPropertyChanged
	{
		
		private static System.ComponentModel.PropertyChangingEventArgs emptyChangingEventArgs = new System.ComponentModel.PropertyChangingEventArgs("");
		
		private sbyte _atOutpost;
		
		private sbyte _atPvE;
		
		private int _groupSize;
		
		private int _level;
		
		private int _mapID;
		
		private int _nameID;
		
		private int _npcID;
		
		private int _npcSpawnID;
		
		private int _plane;
		
		private int _profession;
		
		private float _rotation;
		
		private float _spawnX;
		
		private float _spawnY;
		
		private float _speed;
		
		private int _teamID;
		
		#region Extensibility Method Declarations
		partial void OnCreated();
		
		partial void OnatOutpostChanged();
		
		partial void OnatOutpostChanging(sbyte value);
		
		partial void OnatPvEChanged();
		
		partial void OnatPvEChanging(sbyte value);
		
		partial void OngroupSizeChanged();
		
		partial void OngroupSizeChanging(int value);
		
		partial void OnlevelChanged();
		
		partial void OnlevelChanging(int value);
		
		partial void OnmapIDChanged();
		
		partial void OnmapIDChanging(int value);
		
		partial void OnnameIDChanged();
		
		partial void OnnameIDChanging(int value);
		
		partial void OnnpcIDChanged();
		
		partial void OnnpcIDChanging(int value);
		
		partial void OnnpcSpawnIDChanged();
		
		partial void OnnpcSpawnIDChanging(int value);
		
		partial void OnplaneChanged();
		
		partial void OnplaneChanging(int value);
		
		partial void OnprofessionChanged();
		
		partial void OnprofessionChanging(int value);
		
		partial void OnrotationChanged();
		
		partial void OnrotationChanging(float value);
		
		partial void OnspawnXChanged();
		
		partial void OnspawnXChanging(float value);
		
		partial void OnspawnYChanged();
		
		partial void OnspawnYChanging(float value);
		
		partial void OnspeedChanged();
		
		partial void OnspeedChanging(float value);
		
		partial void OnteamIDChanged();
		
		partial void OnteamIDChanging(int value);
		#endregion
		
		
		public nPcSSpawns()
		{
			this.OnCreated();
		}
		
		[Column(Storage="_atOutpost", Name="AtOutpost", DbType="tinyint(1)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public sbyte atOutpost
		{
			get
			{
				return this._atOutpost;
			}
			set
			{
				if ((_atOutpost != value))
				{
					this.OnatOutpostChanging(value);
					this.SendPropertyChanging();
					this._atOutpost = value;
					this.SendPropertyChanged("atOutpost");
					this.OnatOutpostChanged();
				}
			}
		}
		
		[Column(Storage="_atPvE", Name="AtPvE", DbType="tinyint(1)", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public sbyte atPvE
		{
			get
			{
				return this._atPvE;
			}
			set
			{
				if ((_atPvE != value))
				{
					this.OnatPvEChanging(value);
					this.SendPropertyChanging();
					this._atPvE = value;
					this.SendPropertyChanged("atPvE");
					this.OnatPvEChanged();
				}
			}
		}
		
		[Column(Storage="_groupSize", Name="GroupSize", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int groupSize
		{
			get
			{
				return this._groupSize;
			}
			set
			{
				if ((_groupSize != value))
				{
					this.OngroupSizeChanging(value);
					this.SendPropertyChanging();
					this._groupSize = value;
					this.SendPropertyChanged("groupSize");
					this.OngroupSizeChanged();
				}
			}
		}
		
		[Column(Storage="_level", Name="Level", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int level
		{
			get
			{
				return this._level;
			}
			set
			{
				if ((_level != value))
				{
					this.OnlevelChanging(value);
					this.SendPropertyChanging();
					this._level = value;
					this.SendPropertyChanged("level");
					this.OnlevelChanged();
				}
			}
		}
		
		[Column(Storage="_mapID", Name="MapID", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int mapID
		{
			get
			{
				return this._mapID;
			}
			set
			{
				if ((_mapID != value))
				{
					this.OnmapIDChanging(value);
					this.SendPropertyChanging();
					this._mapID = value;
					this.SendPropertyChanged("mapID");
					this.OnmapIDChanged();
				}
			}
		}
		
		[Column(Storage="_nameID", Name="NameID", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int nameID
		{
			get
			{
				return this._nameID;
			}
			set
			{
				if ((_nameID != value))
				{
					this.OnnameIDChanging(value);
					this.SendPropertyChanging();
					this._nameID = value;
					this.SendPropertyChanged("nameID");
					this.OnnameIDChanged();
				}
			}
		}
		
		[Column(Storage="_npcID", Name="NpcID", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int npcID
		{
			get
			{
				return this._npcID;
			}
			set
			{
				if ((_npcID != value))
				{
					this.OnnpcIDChanging(value);
					this.SendPropertyChanging();
					this._npcID = value;
					this.SendPropertyChanged("npcID");
					this.OnnpcIDChanged();
				}
			}
		}
		
		[Column(Storage="_npcSpawnID", Name="NpcSpawnID", DbType="int", IsPrimaryKey=true, IsDbGenerated=true, AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int npcSpawnID
		{
			get
			{
				return this._npcSpawnID;
			}
			set
			{
				if ((_npcSpawnID != value))
				{
					this.OnnpcSpawnIDChanging(value);
					this.SendPropertyChanging();
					this._npcSpawnID = value;
					this.SendPropertyChanged("npcSpawnID");
					this.OnnpcSpawnIDChanged();
				}
			}
		}
		
		[Column(Storage="_plane", Name="Plane", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int plane
		{
			get
			{
				return this._plane;
			}
			set
			{
				if ((_plane != value))
				{
					this.OnplaneChanging(value);
					this.SendPropertyChanging();
					this._plane = value;
					this.SendPropertyChanged("plane");
					this.OnplaneChanged();
				}
			}
		}
		
		[Column(Storage="_profession", Name="Profession", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int profession
		{
			get
			{
				return this._profession;
			}
			set
			{
				if ((_profession != value))
				{
					this.OnprofessionChanging(value);
					this.SendPropertyChanging();
					this._profession = value;
					this.SendPropertyChanged("profession");
					this.OnprofessionChanged();
				}
			}
		}
		
		[Column(Storage="_rotation", Name="Rotation", DbType="float", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public float rotation
		{
			get
			{
				return this._rotation;
			}
			set
			{
				if ((_rotation != value))
				{
					this.OnrotationChanging(value);
					this.SendPropertyChanging();
					this._rotation = value;
					this.SendPropertyChanged("rotation");
					this.OnrotationChanged();
				}
			}
		}
		
		[Column(Storage="_spawnX", Name="SpawnX", DbType="float", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public float spawnX
		{
			get
			{
				return this._spawnX;
			}
			set
			{
				if ((_spawnX != value))
				{
					this.OnspawnXChanging(value);
					this.SendPropertyChanging();
					this._spawnX = value;
					this.SendPropertyChanged("spawnX");
					this.OnspawnXChanged();
				}
			}
		}
		
		[Column(Storage="_spawnY", Name="SpawnY", DbType="float", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public float spawnY
		{
			get
			{
				return this._spawnY;
			}
			set
			{
				if ((_spawnY != value))
				{
					this.OnspawnYChanging(value);
					this.SendPropertyChanging();
					this._spawnY = value;
					this.SendPropertyChanged("spawnY");
					this.OnspawnYChanged();
				}
			}
		}
		
		[Column(Storage="_speed", Name="Speed", DbType="float", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public float speed
		{
			get
			{
				return this._speed;
			}
			set
			{
				if ((_speed != value))
				{
					this.OnspeedChanging(value);
					this.SendPropertyChanging();
					this._speed = value;
					this.SendPropertyChanged("speed");
					this.OnspeedChanged();
				}
			}
		}
		
		[Column(Storage="_teamID", Name="TeamID", DbType="int", AutoSync=AutoSync.Never, CanBeNull=false)]
		[DebuggerNonUserCode()]
		public int teamID
		{
			get
			{
				return this._teamID;
			}
			set
			{
				if ((_teamID != value))
				{
					this.OnteamIDChanging(value);
					this.SendPropertyChanging();
					this._teamID = value;
					this.SendPropertyChanged("teamID");
					this.OnteamIDChanged();
				}
			}
		}
		
		public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			System.ComponentModel.PropertyChangingEventHandler h = this.PropertyChanging;
			if ((h != null))
			{
				h(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(string propertyName)
		{
			System.ComponentModel.PropertyChangedEventHandler h = this.PropertyChanged;
			if ((h != null))
			{
				h(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
