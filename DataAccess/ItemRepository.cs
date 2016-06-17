
using System;
using Debenhams.Models;
using SQLite;
using System.Collections.Generic;
using Android.Graphics;
using System.Linq;

namespace Debenhams.DataAccess
{
	public class ItemRepository
	{

		public ItemRepository ()
		{
		}

		#region >>>>>>>>>>POLIST<<<<<<<<<<

		public static tblPoList ChkPoListExist(string receiver_num,string division)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblPoList> ()
					.Where(t=>t.receiver_num==receiver_num && t.division==division)
					.SingleOrDefault ();
			}
		}

		public static long UserLogin(tblUser item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}

		public static long AddRPoList(string po_num, string receiver_num, string division_id, string division,string status)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
				(new tblPoList
					{
						po_num = po_num,
						receiver_num = receiver_num,
						division_id = division_id,
						division = division,
						slot_num = "",
						status = status
					}
				);
			}
		}
			
		public static long AddRPoListDetail(string receiver_num, string division_id,string upc,string description,string oqty,string rqty,string status)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
				(new tblPoListDetail
					{
						receiver_num = receiver_num,
						division_id = division_id,
						upc=upc,
						description = description,
						oqty = oqty,
						rqty = rqty,
						status=status
					}
				);
			}
		}

		#endregion

		#region >>>>>>>>>>PICKING TL LIST<<<<<<<<<<

		public static List<tblPickingListDetail> GetPickingListDetail(string move_doc)
		{
			tblPickingListDetail itemlist = new tblPickingListDetail();
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblPickingListDetail> ()
					.Where (t => t.move_doc == move_doc)
					.OrderBy (t => t.dept).ToList()
					.OrderBy (t=> t.status).ToList();
			}
		}

		public static tblPickingListDetail GetPTLUPC(string move_doc,string upc)
		{
			upc = upc.Substring (0, Convert.ToInt32 (upc.Length-1));
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblPickingListDetail> ()
					.Where (t => t.move_doc==move_doc && t.upc==upc).SingleOrDefault ();
			}
		}

		public static long UpdatePTLListDetail(tblPickingListDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}


		#endregion

		#region >>>>>>>>>>BOXLIST<<<<<<<<<<

		public static List<tblBox> GetBoxList()
		{
			tblBox itemlist = new tblBox();
			using (var database = WMSDatabase.NewConnection ())
			{
				return database
					.Table<tblBox>()
					.OrderByDescending(t=> t.id)
					.ToList ();
			}
		}

		public static tblBox GetPBoxCount(string boxcode)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblBox> ()
					.Where (t => t.box_code==boxcode)
					.SingleOrDefault ();
			}
		}

		public static tblBox GetPBoxCode(string boxcode)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblBox> ()
					.Where (t => t.box_code==boxcode)
					.SingleOrDefault ();
			}
		}

		public static long AddPBox(string box_code, string move_doc,string number,string total)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblBox
						{
							box_code = box_code,
							move_doc = move_doc,
							number=number,
							total = total
						}
					);
			}
		}
		#region >>>>>>>>>>BOXLIST_DETAILS<<<<<<<<<<

		public static tblBoxDetail ChkBoxDetailUPC(string box_code,string move_doc,string upc)
		{
			using (var database = WMSDatabase.NewConnection ()) 
			{
				return database
					.Table<tblBoxDetail> ()
					.Where (t => t.box_code==box_code && t.move_doc==move_doc && t.upc==upc)
					.SingleOrDefault ();
			}
		}

		public static long AddBoxDetail(string box_code, string move_doc,string upc,string rqty)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				return database.Insert
					(new tblBoxDetail
						{
							box_code = box_code,
							move_doc = move_doc,
							upc=upc,
							rqty = rqty
						}
					);
			}
		}

		public static long UpdateBoxDetail(tblBoxDetail item)
		{
			using (var database = WMSDatabase.NewConnection ())
			{
				database.Update(item);
				return item.id;
			}
		}

		#endregion
		#endregion
	}
}

