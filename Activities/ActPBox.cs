
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Debenhams.DataAccess;

namespace Debenhams.Activities
{
	[Activity (Label = "ActPBox",Theme = "@android:style/Theme.Light.NoTitleBar")]			
	public class ActPBox : Activity
	{
		private EditText txttl_no, store_name;
		private TextView lbltl_no, lblboxnum;
		private Spinner spnbox;
		private Button btncreatebox, btnaddbox, btndonebox;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.LayPBox);

			txttl_no = FindViewById<EditText> (Resource.Id.txttl_no);
			store_name = FindViewById<EditText> (Resource.Id.txtstore_name);

			lbltl_no = FindViewById<TextView> (Resource.Id.lbltl_no);
			lblboxnum = FindViewById<TextView> (Resource.Id.lblboxnum);

			btncreatebox = FindViewById<Button> (Resource.Id.btncreatebox);
			btnaddbox = FindViewById<Button> (Resource.Id.btnaddbox);
			btndonebox = FindViewById<Button> (Resource.Id.btndonebox);

			spnbox = FindViewById<Spinner> (Resource.Id.spnbox);

			txttl_no.Text = Intent.GetStringExtra ("move_doc");
			store_name.Text = Intent.GetStringExtra ("store_name");

			var boxlist = ItemRepository.GetBoxList ().Select (t => t.move_doc).ToList ();
			var adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleSpinnerItem, boxlist);
			adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spnbox.Adapter = adapter;

			spnbox.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spnbox_clicked);

			btncreatebox.Click += new EventHandler (btncreatebox_Clicked);
			btnaddbox.Click += new EventHandler (btnaddbox_Clicked);
			btndonebox.Click += new EventHandler (btndonebox_Clicked);

			

		}

		private void spnbox_clicked(object sender,EventArgs e )
		{
			var box = ItemRepository.GetPBoxCode (spnbox.SelectedItem.ToString ());
			var boxcount= ItemRepository.GetPBoxCode (spnbox.SelectedItem.ToString ());
			lbltl_no.Text = box.move_doc;
			lblboxnum.Text = box.number + " of " + box.total;
		}

		private void  btnaddbox_Clicked(object sender,EventArgs e)
		{
			var builder = new AlertDialog.Builder(this);
			builder.SetTitle ("Debenhams");
			builder.SetMessage ("ADDING BOX.." +
				"\nBox Code: "+ lbltl_no.Text + 
				"\nTL No: "+ lbltl_no.Text +
				"\nBox No: "+ lbltl_no.Text);
			builder.SetPositiveButton("Yes", delegate { builder.Dispose(); });
			builder.SetNegativeButton("No",delegate { builder.Dispose(); });
			builder.Show ();
		}

		private void  btncreatebox_Clicked(object sender,EventArgs e)
		{
			var builder = new AlertDialog.Builder(this);
			builder.SetTitle ("Debenhams");
			builder.SetMessage ("CREATING BOX.." +
				"\nBox Code: "+ lbltl_no.Text + 
				"\nTL No: "+ txttl_no.Text +
				"\nBox No: 1 of 1");
			builder.SetPositiveButton("Yes", delegate { builder.Dispose(); });
			builder.SetNegativeButton("No",delegate { builder.Dispose(); });
			builder.Show ();
		}

		private void  btndonebox_Clicked(object sender,EventArgs e)
		{
			var intent = new Intent();
			intent.SetClass(this, typeof(ActPTLScanList));
			intent.PutExtra("move_doc", Intent.GetStringExtra ("move_doc"));
			intent.PutExtra("store_name",Intent.GetStringExtra ("store_name"));
			intent.PutExtra("box_code", spnbox.SelectedItem.ToString ());
			StartActivity(intent);
		}

	}
}

