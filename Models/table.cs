using SQLite;

namespace Debenhams.Models
{
	public class tblPoList
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string po_num { get; set; }
		public string receiver_num { get; set; }
		public string division_id { get; set; }
		public string division { get; set; }
		public string slot_num { get; set; }
		public string status { get; set; }
	}

	public class tblPoListDetail
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string receiver_num { get; set; }
		public string division_id { get; set; }
		public string upc { get; set; }
		public string description { get; set; }
		public string oqty { get; set; }
		public string rqty { get; set; }
		public string status { get; set; }
	}
		
	public class tblUser
	{
		[PrimaryKey]
		public long id { get; set; }
		public string userid { get; set; }
		public string username { get; set; }
		public string password { get; set; }
		public string fname { get; set; }
		public string lname { get; set; }
		public string token { get; set; }
		public string status { get; set; }
	}

	public class tblConnectionURL
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string url { get; set; }
	}

	public class tblProductList
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string upc { get; set; }
		public string descr { get; set; }
	}

	public class tblPickingList
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string move_doc { get; set; }
		public string store_id { get; set; }
		public string store_name { get; set; }
		public string status { get; set; }
	}

	public class tblPickingListDetail
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string move_doc { get; set; }
		public string picking_id { get; set; }
		public string upc { get; set; }
		public string sku { get; set; }
		public string dept { get; set; }
		public string style { get; set; }
		public string descr { get; set; }
		public string oqty { get; set; }
		public string rqty { get; set; }
		public string status { get; set; }
	}

	public class tblBox
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string box_code { get; set; }
		public string move_doc { get; set; }
		public string number { get; set; }
		public string total { get; set; }
	}

	public class tblBoxDetail
	{
		[PrimaryKey, AutoIncrement]
		public long id { get; set; }
		public string box_code { get; set; }
		public string move_doc { get; set; }
		public string upc { get; set; }
		public string rqty { get; set; }
	}
		
}

