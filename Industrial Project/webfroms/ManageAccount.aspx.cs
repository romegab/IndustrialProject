﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Industrial_Project.webfroms
{

    public partial class ManageAccount : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["username"] == null)
            {
                Response.Redirect("Login.aspx");
            }
            if (Session["role"].ToString() != "Admin")
            {
                UploadButton.Attributes.Add("style", "display:none");
            }
            userName.Attributes.Add("placeholder", "Enter new username");
            userPassword.Attributes["placeholder"] = "Enter new password";
        }

        /// <summary>
        /// Logs out the user and deletes the session.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LogOut_click(object sender, EventArgs e)
        {
            Session["username"] = null;
            Response.Redirect("Login.aspx");
        }

        /// <summary>
        /// Redirects you to the upload page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Upload_Click(object sender, EventArgs e)
        {

            Response.Redirect("Upload.aspx");
        }

        /// <summary>
        ///  Redirects to the appropriate page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AccClick(object sender, EventArgs e)
        {
            if (Session["role"].ToString() == "User") Response.Redirect("ManageAccount.aspx");
            if (Session["role"].ToString() == "Admin") Response.Redirect("UserAlteration.aspx");
        }

        /// <summary>
        /// Updates the details of the account.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Update_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection();
            string connString = ConfigurationManager.ConnectionStrings["Connection"].ConnectionString;
            conn.ConnectionString = connString;
            conn.Open();

            if (String.IsNullOrEmpty(userName.Text) || String.IsNullOrEmpty(userPassword.Text))
            {
                Label1.Visible = true;
                Label1.Text = "Fill all the user details";
            }
            else if (userName.Text == Session["username"].ToString())
            {
                SqlCommand command = new SqlCommand("UpdateUserDetails", conn);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                SqlParameter OldUsername = new SqlParameter("oldUsername", Session["username"].ToString());
                SqlParameter newUsername = new SqlParameter("newUsername", userName.Text);
                SqlParameter password = new SqlParameter("password", Hash(userPassword.Text));
                SqlParameter role = new SqlParameter("role", Session["role"]);
                command.Parameters.Add(OldUsername);
                command.Parameters.Add(newUsername);
                command.Parameters.Add(password);
                command.Parameters.Add(role);

                int result = command.ExecuteNonQuery();
                if (result != 0)
                {
                    Session["username"] = userName.Text;
                    Label1.Visible = true;
                    Label1.Text = "Successfully updated your details";

                }
                else
                {
                    Label1.Visible = true;
                    Label1.Text = "There was a problem with the update of your details";
                }
            }
            else
            {
                SqlCommand com = new SqlCommand("CheckUsername", conn);
                com.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter username = new SqlParameter("username", userName.Text);
                com.Parameters.Add(username);

                SqlDataReader rd = com.ExecuteReader();
                if (!rd.HasRows)
                {
                    rd.Close();
                    SqlCommand command = new SqlCommand("UpdateUserDetails", conn);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlParameter OldUsername = new SqlParameter("oldUsername", Session["username"].ToString());
                    SqlParameter newUsername = new SqlParameter("newUsername", userName.Text);
                    SqlParameter password = new SqlParameter("password", Hash(userPassword.Text));
                    SqlParameter role = new SqlParameter("role", Session["role"]);
                    command.Parameters.Add(OldUsername);
                    command.Parameters.Add(newUsername);
                    command.Parameters.Add(password);
                    command.Parameters.Add(role);

                    int result = command.ExecuteNonQuery();
                    if (result != 0)
                    {
                        Session["username"] = userName.Text;
                        Label1.Visible = true;
                        Label1.Text = "Successfully updated your details";

                    }
                    else
                    {
                        Label1.Visible = true;
                        Label1.Text = "There was a problem with the update of your details";
                    }

                }
                else
                {

                    Label1.Visible = true;
                    Label1.Text = "The username already exists";
                }
            }
            conn.Close();
            conn.Dispose();

        }

        /// <summary>
        ///  Hashes the password.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static string Hash(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("x2")); //x2 for lowercase hashes
                }

                return sb.ToString();
            }
        }
    }
}

