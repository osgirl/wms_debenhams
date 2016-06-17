using System;
using System.Threading.Tasks;
using System.Json;
using System.Net;
using System.IO;
using System.Net.Http;
using Android.App;
using Debenhams.DataAccess;
using Debenhams.Models;
using SQLite;
using System.Collections.Generic;
using Android.Graphics;
using System.Linq;
using Debenhams;
namespace Debenhams.ApiConnection
{
	public class ApiConnection1
	{
		tblUser user1 = new tblUser();

		public ApiConnection1 ()
		{
		
		}

		public static async Task<bool> ApiPoRList(string url)
		{
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr)
						{
							string status="Open";
							string receiver_num = Convert.ToString ((int)item ["receiver_no"]);
							string division_id = Convert.ToString ((int)item ["division_id"]);
							var po = ItemRepository.ChkPoListExist (receiver_num, division_id);
							if (po == null) 
							{
								ItemRepository.AddRPoList (Convert.ToString ((int)item ["purchase_order_no"]),receiver_num,
								division_id,Convert.ToString ((int)item ["division"]),status);
								await ApiPoRListDetail (GlobalVariables.GlobalUrl + "/RPoListDetail/" + receiver_num + "/" + division_id);
							}
						}
					}
				}
			}
			return result;
		}

		public static async Task<bool> ApiPoRListDetail(string url)
		{
			bool result = false;
	
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr) 
						{
							string status="Open";
							string receiver_num = Convert.ToString ((int)item ["receiver_no"]);
							string division_id = Convert.ToString ((int)item ["division"]);
							if ((int)item ["po_status"] == 3)status = "In Process";else if((int)item ["po_status"] == 4) status = "Done";

							Convert.ToString ((int)item ["quantity_ordered"]);
							Convert.ToString ((int)item ["quantity_delivered"]);
							ItemRepository.AddRPoListDetail (Convert.ToString ((int)item ["receiver_no"]),Convert.ToString((int)item ["division"]),
								(string)item ["upc"],(string)item ["short_description"],Convert.ToString((int)item ["quantity_ordered"]),
								Convert.ToString((int)item ["quantity_delivered"]),status
							);
						}
					}
				}
			}
			return result;
		}

		public static async Task<bool> ApiDebsUpdateData(string url)
		{
			bool result = false;

			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{

				}
			}
			return result;
		}
			
		public static async Task<bool> UserLogin(string url)
		{
			tblUser tbluser = new tblUser ();
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr) 
						{
							tbluser.id= 1;
							tbluser.userid=Convert.ToString( (int)item["id"]);
							tbluser.username=item ["username"];
							tbluser.password=item ["password"];
							tbluser.fname=item ["fname"];
							tbluser.lname=item ["lname"];
							tbluser.token=item ["token"];
							tbluser.status="1";
							GlobalVariables.GlobalUserid=Convert.ToString( (int)item["id"]);
							ItemRepository.UserLogin (tbluser);
						}
					}
				}
			}
			return result;
		}



		#region >>>>>>>>>>Picking<<<<<<<<<<

		public static async Task<bool> ApiPTLList(string url)
		{
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr)
						{
							using (var database = WMSDatabase.NewConnection ())
							{
								 database.Insert
								(new  tblPickingList
									{
										move_doc = Convert.ToString((int)item["move_doc_number"]),
										store_id =item["store_code"],
										store_name = item["store_name"],
										status = "Open"
									}
								);
							}
							await ApiPTLListDetail (GlobalVariables.GlobalUrl + "/PTLListDetail/" + Convert.ToString((int)item["move_doc_number"]));
						}
					}
				}
			}
			return result;
		}

		public static async Task<bool> ApiPTLListDetail(string url)
		{
			bool result = false;
			var request = System.Net.WebRequest.Create(url) as HttpWebRequest;
			if (request != null)
			{
				request.Method = "GET";
				request.ServicePoint.Expect100Continue = false;
				request.Timeout = 20000;
				request.ContentType = "application/json";
				using (WebResponse response = await request.GetResponseAsync ()) 
				{
					using (Stream stream = response.GetResponseStream ()) 
					{
						string x = JsonObject.Load(stream).ToString();
						JsonObject jObj = (JsonObject)JsonObject.Parse(x);
						JsonArray jArr = (JsonArray)jObj["result"];
						foreach (var item in jArr)
						{
							using (var database = WMSDatabase.NewConnection ())
							{
								database.Insert
								(new  tblPickingListDetail
									{
										move_doc = Convert.ToString((int)item["move_doc_number"]),
										picking_id= Convert.ToString((int)item["id"]),
										upc = item["upc"],
										sku = item["sku"],
										dept = item["dept"],
										style = item["short_description"],
										descr = item["short_description"],
										oqty = Convert.ToString((int)item["quantity_to_pick"]),
										rqty = Convert.ToString((int)item["moved_qty"]),
										status = "Open"
									}
								);
							}
						}
					}
				}
			}
			return result;
		}

		#endregion

	}
}

