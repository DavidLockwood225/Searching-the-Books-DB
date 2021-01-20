/*David Lockwood 
 *1/19/2021
 *Purpose: Access the Books database and display a book's author, title, and publisher, as well as search the database for books by particular authors using first initial of their last name.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace Searching_the_Books_DB_Project
{
    public partial class frmBooks : Form
    {
        SqlConnection booksConnection;
        String SQLAll;
        Button[] btnRolodex = new Button[26];
        public frmBooks()
        {
            InitializeComponent();
        }

        private void frmBooks_Load(object sender, EventArgs e)
        {
            booksConnection = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;"
                                               + "AttachDbFilename=" + Path.GetFullPath("SQLBooksDB.mdf")
                                               + ";Integrated Security=True;"
                                               + "Connect Timeout=30;");
            booksConnection.Open();

            int w, lStart, l, t;
            int buttonHeight = 33;

            w = Convert.ToInt32(this.ClientSize.Width / 14);
            lStart = Convert.ToInt32(0.5 * (this.ClientSize.Width - 13 * w));
            l = lStart;
            t = grdBooks.Top + grdBooks.Height + 2;

            for (int i = 0; i < 26; i++)
            {
                btnRolodex[i] = new Button();
                btnRolodex[i].TabStop = false;
                btnRolodex[i].Text = ((char)(65 + i)).ToString();
                btnRolodex[i].Width = w;
                btnRolodex[i].Height = buttonHeight;
                btnRolodex[i].Left = l;
                btnRolodex[i].Top = t;
                btnRolodex[i].BackColor = Color.Blue;
                btnRolodex[i].ForeColor = Color.White;
                this.Controls.Add(btnRolodex[i]);
                btnRolodex[i].Click += new System.EventHandler(this.btnSQL_Click);

                l += w;
                if (i == 12)
                {
                    l = lStart;
                    t += buttonHeight;
                }
            }

            SQLAll = "SELECT Authors.Author,Titles.Title,Publishers.Company_Name ";
            SQLAll += "FROM Authors, Titles, Publishers, Title_Author ";
            SQLAll += "WHERE Titles.ISBN = Title_Author.ISBN ";
            SQLAll += "AND Authors.Au_ID = Title_Author.Au_ID ";
            SQLAll += "AND Titles.PubID = Publishers.PubID ";

            this.Show();
            btnAll.PerformClick();
        }

        private void frmBooks_FormClosing(object sender, FormClosingEventArgs e)
        {
            booksConnection.Close();
            booksConnection.Dispose();
        }
        private void btnSQL_Click(object sender, EventArgs e)
        {
            SqlCommand resultsCommand = null;
            SqlDataAdapter resultsAdapter = new SqlDataAdapter();
            DataTable resultsTable = new DataTable();
            String SQLStatement;
            Button buttonClicked = (Button) sender;
            switch (buttonClicked.Text)
            {
                case "Show All Records":
                    SQLStatement = SQLAll;
                    break;
                case "Z":
                    SQLStatement = SQLAll + "AND Authors.Author > 'Z' ";
                    break;
                default:
                    int index = (int)(Convert.ToChar(buttonClicked.Text)) - 65;
                    SQLStatement = SQLAll + "AND Authors.Author > '" + btnRolodex[index].Text + " ' ";
                    break;
            }
            SQLStatement += "ORDER BY Authors.Author";

            try
            {
                resultsCommand = new SqlCommand(SQLStatement, booksConnection);
                resultsAdapter.SelectCommand = resultsCommand;
                resultsAdapter.Fill(resultsTable);
                grdBooks.DataSource = resultsTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error in Processing SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            resultsCommand.Dispose();
            resultsAdapter.Dispose();
            resultsTable.Dispose();
        }
    }
}
