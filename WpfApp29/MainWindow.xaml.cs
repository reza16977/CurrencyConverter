using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
namespace WpfApp29
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection con = new SqlConnection(); // create object for sqlConnection
        SqlCommand cmd = new SqlCommand();       // create object for sqlCommand
        SqlDataAdapter da = new SqlDataAdapter(); // create object for sqlDataAdapter

        private int CurrencyId = 0;
        private double FromAmount = 0;
        private double ToAmount = 0;

        

        public MainWindow()
        {
            InitializeComponent();
            BindCurrency();
            BindCurrency();
            GetData();
        }

        public void myCon()
        {
            //database connection string
            string Conn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            con = new SqlConnection(Conn);
            con.Open();
        }

        private void BindCurrency()
        {
            myCon();
            //create an object for datatable
            DataTable dt = new DataTable();

            //write query to get data from currency_master table
            cmd = new SqlCommand("Select Id, CurrencyName from Currency_Master",con);
            //Command type define which type of command we use for write a query
            cmd.CommandType = CommandType.Text;

            //it is accepting a parameter that contains the command text of the object's select commond property
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            //create an object for dataRow
            DataRow newRow = dt.NewRow();
            //Assign value to id column and  currencyName column
            newRow["Id"] = 0;
            newRow["CurrencyName"] = "--SELECT--";
            //insert a new row in dt with the data at a 0 position
            dt.Rows.InsertAt(newRow, 0);

            //dt is not null and rows count greater than 0 
            if(dt!=null&&dt.Rows.Count>0)
            {
                //assign the datatable data to fromCurrecny combobox using ItemSource property
                cmbFromCurrency.ItemsSource = dt.DefaultView;
                //Assign the datatable data to toCurrecny combobox using ItemSource property
                cmbToCurrency.ItemsSource = dt.DefaultView;
            }
            con.Close();

            cmbFromCurrency.DisplayMemberPath = "CurrencyName";
            cmbFromCurrency.SelectedValuePath = "Id";
            cmbFromCurrency.SelectedIndex = 0;

         
            cmbToCurrency.DisplayMemberPath = "CurrencyName";
            cmbToCurrency.SelectedValuePath = "Id";
            cmbToCurrency.SelectedIndex = 0;

        }

        private void NumberValidationTextBox(Object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

        }

        private void Convert_Click(Object sender, RoutedEventArgs e)
        {
            double ConvertedValue;

            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                MessageBox.Show("Please enter currency", " Inforamtion", MessageBoxButton.OK, MessageBoxImage.Information);
                txtCurrency.Focus();
                return;
            }

            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please select currency from", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                cmbFromCurrency.Focus();
                return;
            }
            //else if currency to is not selected or select default --select
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please select currency to", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                
                //set focus on the To ComboBox
                cmbToCurrency.Focus();
                return;
            }

            if(cmbFromCurrency.Text== cmbToCurrency.Text)
            {
                ConvertedValue = double.Parse(txtCurrency.Text);
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N2");
            }
            else
            {
                ConvertedValue = (double.Parse(cmbFromCurrency.SelectedValue.ToString()) * 
                    double.Parse(txtCurrency.Text)) / double.Parse(cmbToCurrency.
                    SelectedValue.ToString());

                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
        }

        private void Clear_Click(Object sender, RoutedEventArgs e)
        {
            ClearControls();
        }

        private void ClearControls()
        {
            txtCurrency.Text = String.Empty;
            if (cmbFromCurrency.Items.Count > 0) cmbFromCurrency.SelectedIndex = 0;
            if (cmbToCurrency.Items.Count > 0) cmbToCurrency.SelectedIndex = 0;
            lblCurrency.Content = "";
            txtCurrency.Focus();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(txtAmount.Text ==null || txtAmount.Text.Trim() =="")
                {
                    MessageBox.Show("Please enter amount", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtAmount.Focus();
                    return;
                }
                else if(txtCurrencyName.Text == null || txtCurrencyName.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter currency name", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCurrencyName.Focus();
                    return;
                }
                else
                {
                    if(CurrencyId>0) //code for update button .here check currencyId greater that zero than it is go for update
                    {
                        if (MessageBox.Show("Are you sure you want to update ?","Information",MessageBoxButton.YesNo,MessageBoxImage.Question) == MessageBoxResult.Yes) //showConfirmation message

                        {
                            myCon();
                            DataTable dt = new DataTable();
                            //update query record update using Id
                            cmd = new SqlCommand("UPDATE Currency_Master SET Amount =@Amount, CurrencyName = @CurrencyName where Id=@Id",con);
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Id", CurrencyId);
                            cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
                            cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            cmd.ExecuteNonQuery();
                            con.Close();

                            MessageBox.Show("Data updated successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }

                    else //save button code
                    {
                        if(MessageBox.Show("Are you sure you want to save ? ","Information",MessageBoxButton.YesNo,MessageBoxImage.Question)==MessageBoxResult.Yes)
                        {
                            myCon();
                            cmd = new SqlCommand("insert into Currency_Master(Amount,CurrencyName) values(@Amount,@CurrencyName)",con);
                            cmd.CommandType= CommandType.Text;
                            cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
                            cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            cmd.ExecuteNonQuery();
                            con.Close();

                            MessageBox.Show("Data saved successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    ClearMaster();
                }


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, " Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearMaster()
        {
            try
            {
                txtAmount.Text = String.Empty;
                txtCurrencyName.Text = String.Empty;
                btnSave.Content = "Save";
                GetData();
                CurrencyId = 0;
                BindCurrency();
                txtAmount.Focus();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GetData()
        {
            myCon(); //mycon() method is used for connect with database and open database conncection
            DataTable dt = new DataTable(); // create datatable object which is usable in c#
            cmd = new SqlCommand("select * from Currency_Master", con); //write sql query for get data from database table
            cmd.CommandType = CommandType.Text;
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            if(dt!=null && dt.Rows.Count>0)
            {
                dgvCurrency.ItemsSource = dt.DefaultView; //Assign datatable data to dgvCurrency using itemsSource property
            }
            else
            {
                dgvCurrency.ItemsSource = null;
            }
            con.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                ClearMaster();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgvCurrency_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                DataGrid grd = (DataGrid)sender;  //create object for dataGrid
                DataRowView row_selected = grd.CurrentItem as DataRowView; //create objcet for dataRowView

                if(row_selected != null)
                {
                    if(dgvCurrency.Items.Count>0)
                    {
                        if(grd.SelectedCells.Count>0)
                        {
                            CurrencyId = Int32.Parse(row_selected["Id"].ToString());

                            if (grd.SelectedCells[0].Column.DisplayIndex==0)  //displayindex is equal to zero than it is edit cell
                            {
                                txtAmount.Text = row_selected["Amount"].ToString();
                                txtCurrency.Text = row_selected["CurrencyName"].ToString();
                                btnSave.Content = "Update";  //change save button text to 'Update'
                            }
                            if (grd.SelectedCells[0].Column.DisplayIndex ==1)
                            {
                                if(MessageBox.Show("Are you sure you want to delete ?","Information",MessageBoxButton.YesNo,MessageBoxImage.Question)==MessageBoxResult.Yes)
                                {
                                    myCon();
                                    DataTable dt = new DataTable();
                                    cmd = new SqlCommand("delete from Currency_Master where Id = @Id",con);
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.AddWithValue("@Id", CurrencyId);
                                    cmd.ExecuteNonQuery();
                                    con.Close();

                                    MessageBox.Show("Data deleted successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                    ClearMaster();

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }


}
