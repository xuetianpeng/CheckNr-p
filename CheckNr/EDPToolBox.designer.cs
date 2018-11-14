﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace CheckNr
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="EDPToolBox")]
	public partial class EDPToolBoxDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region 可扩展性方法定义
    partial void OnCreated();
    partial void InsertT_CheckNr(T_CheckNr instance);
    partial void UpdateT_CheckNr(T_CheckNr instance);
    partial void DeleteT_CheckNr(T_CheckNr instance);
    #endregion
		
		public EDPToolBoxDataContext() : 
				base(global::CheckNr.Properties.Settings.Default.EDPToolBoxConnectionString1, mappingSource)
		{
			OnCreated();
		}
		
		public EDPToolBoxDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public EDPToolBoxDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public EDPToolBoxDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public EDPToolBoxDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<T_CheckNr> T_CheckNr
		{
			get
			{
				return this.GetTable<T_CheckNr>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.T_CheckNr")]
	public partial class T_CheckNr : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _STID;
		
		private string _STName;
		
		private string _Pw;
		
		private System.Nullable<System.DateTime> _LastTime;
		
		private string _LastMsg;
		
    #region 可扩展性方法定义
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnSTIDChanging(string value);
    partial void OnSTIDChanged();
    partial void OnSTNameChanging(string value);
    partial void OnSTNameChanged();
    partial void OnPwChanging(string value);
    partial void OnPwChanged();
    partial void OnLastTimeChanging(System.Nullable<System.DateTime> value);
    partial void OnLastTimeChanged();
    partial void OnLastMsgChanging(string value);
    partial void OnLastMsgChanged();
    #endregion
		
		public T_CheckNr()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_STID", DbType="VarChar(50) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string STID
		{
			get
			{
				return this._STID;
			}
			set
			{
				if ((this._STID != value))
				{
					this.OnSTIDChanging(value);
					this.SendPropertyChanging();
					this._STID = value;
					this.SendPropertyChanged("STID");
					this.OnSTIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_STName", DbType="VarChar(100) NOT NULL", CanBeNull=false)]
		public string STName
		{
			get
			{
				return this._STName;
			}
			set
			{
				if ((this._STName != value))
				{
					this.OnSTNameChanging(value);
					this.SendPropertyChanging();
					this._STName = value;
					this.SendPropertyChanged("STName");
					this.OnSTNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Pw", DbType="VarChar(100) NOT NULL", CanBeNull=false)]
		public string Pw
		{
			get
			{
				return this._Pw;
			}
			set
			{
				if ((this._Pw != value))
				{
					this.OnPwChanging(value);
					this.SendPropertyChanging();
					this._Pw = value;
					this.SendPropertyChanged("Pw");
					this.OnPwChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LastTime", DbType="DateTime")]
		public System.Nullable<System.DateTime> LastTime
		{
			get
			{
				return this._LastTime;
			}
			set
			{
				if ((this._LastTime != value))
				{
					this.OnLastTimeChanging(value);
					this.SendPropertyChanging();
					this._LastTime = value;
					this.SendPropertyChanged("LastTime");
					this.OnLastTimeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LastMsg", DbType="Text", UpdateCheck=UpdateCheck.Never)]
		public string LastMsg
		{
			get
			{
				return this._LastMsg;
			}
			set
			{
				if ((this._LastMsg != value))
				{
					this.OnLastMsgChanging(value);
					this.SendPropertyChanging();
					this._LastMsg = value;
					this.SendPropertyChanged("LastMsg");
					this.OnLastMsgChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
