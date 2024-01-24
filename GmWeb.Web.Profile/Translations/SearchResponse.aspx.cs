using System.Web.SessionState;
using System.Configuration;
using System.Web.Caching;
using System.Collections.Specialized;
using System.Web.UI.WebControls.WebParts;
using System.Data;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.Profile;
using System.Web;
using System.Linq;
using System.Web.Security;
using System.Collections.Generic;
using System.Collections;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Text.RegularExpressions;
using System;
using System.Xml.Linq;
using System.Xml;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace GmWeb.Web.ProfileApp
{
    partial class SearchResponse : System.Web.UI.Page
    {
        private DataComponent _dataComponent;
        private string connectionString = ConfigurationManager.ConnectionStrings["CURRENT_INSTANCE_DB"].ConnectionString;


        // Page Load
        protected void Page_Load(object sender, System.EventArgs e)
        {

            // Transfer to the logon screen if they're not logged in.
            if (Session.Count == 0 || Session["UserEmailAddress"] == null)
                Response.Redirect("Logon.aspx");

            SqlCommand cmd = new SqlCommand();

            string dmlSearchType = "";
            double dmlSearchLatitude;
            double dmlSearchLongitude;
            string latLngPoly = "";
            // Dim intSearchRadius As Int32
            decimal intSearchRadius;
            string shpName = "";
            Int32 shpID;
            Int32 firstShpID;
            Int32 secondShpID;
            Int32 agencyID;
            string requestDataType = System.Convert.ToString(Request.QueryString["requestDataType"]);
            string userEmailAddress = "";
            string city = "";
            string shpCoords = "";

            // Client Services
            Int32 clientID;
            Int32 categoryID;
            Int32 clientCategoryID;
            Int32 agencyIDParent;
            Int32 clientCategoryDateID;

            // Build command parameters list
            if (!string.IsNullOrEmpty(Request.QueryString["searchType"]))
                dmlSearchType = System.Convert.ToString(Request.QueryString["searchType"]);
            if (!string.IsNullOrEmpty(Request.QueryString["lat"]))
            {
                dmlSearchLatitude = System.Convert.ToDouble(Request.QueryString["lat"]);
                cmd.Parameters.AddWithValue("@dmlLat", dmlSearchLatitude);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["lng"]))
            {
                dmlSearchLongitude = System.Convert.ToDouble(Request.QueryString["lng"]);
                cmd.Parameters.AddWithValue("@dmlLng", dmlSearchLongitude);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["latLngPoly"]))
            {
                latLngPoly = System.Convert.ToString(Request.QueryString["latLngPoly"]);
                cmd.Parameters.AddWithValue("@LatLng", latLngPoly);
                cmd.Parameters.AddWithValue("@ShpUserPolygonID", 0);
            }

            if (!string.IsNullOrEmpty(Request.QueryString["radius"]))
            {
                intSearchRadius = System.Convert.ToDecimal(Request.QueryString["radius"]);
                cmd.Parameters.AddWithValue("@intRadius", intSearchRadius);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["shpName"]))
            {
                shpName = System.Convert.ToString(Request.QueryString["shpName"]);
                cmd.Parameters.AddWithValue("@shpName", shpName);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["shpID"]))
            {
                shpID = System.Convert.ToInt32(Request.QueryString["shpID"]);
                cmd.Parameters.AddWithValue("@shpID", shpID);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["firstShpID"]))
            {
                firstShpID = System.Convert.ToInt32(Request.QueryString["firstShpID"]);
                cmd.Parameters.AddWithValue("@firstShpID", firstShpID);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["secondShpID"]))
            {
                secondShpID = System.Convert.ToInt32(Request.QueryString["secondShpID"]);
                cmd.Parameters.AddWithValue("@secondShpID", secondShpID);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["agencyID"]))
            {
                agencyID = System.Convert.ToInt32(Request.QueryString["agencyID"]);
                cmd.Parameters.AddWithValue("@agencyID", agencyID);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["userEmailAddress"]))
            {
                userEmailAddress = System.Convert.ToString(Request.QueryString["userEmailAddress"]);
                cmd.Parameters.AddWithValue("@UserEmailAddress", userEmailAddress);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["city"]))
            {
                city = System.Convert.ToString(Request.QueryString["city"]);
                cmd.Parameters.AddWithValue("@City", city);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["shpCoords"]))
            {
                shpCoords = System.Convert.ToString(Request.QueryString["shpCoords"]);
                cmd.Parameters.AddWithValue("@shpCoords", shpCoords);
            }

            // Client Services
            if (!string.IsNullOrEmpty(Request.QueryString["clientID"]))
            {
                clientID = System.Convert.ToInt32(Request.QueryString["clientID"]);
                cmd.Parameters.AddWithValue("@ClientID", clientID);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["categoryID"]))
            {
                categoryID = System.Convert.ToInt32(Request.QueryString["categoryID"]);
                cmd.Parameters.AddWithValue("@CategoryID", categoryID);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["clientCategoryID"]))
            {
                clientCategoryID = System.Convert.ToInt32(Request.QueryString["clientCategoryID"]);
                cmd.Parameters.AddWithValue("@ClientCategoryID", clientCategoryID);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["agencyIDParent"]))
            {
                agencyIDParent = System.Convert.ToInt32(Request.QueryString["agencyIDParent"]);
                cmd.Parameters.AddWithValue("@AgencyIDParent", agencyIDParent);
            }
            if (!string.IsNullOrEmpty(Request.QueryString["clientCategoryDateID"]))
            {
                clientCategoryDateID = System.Convert.ToInt32(Request.QueryString["clientCategoryDateID"]);
                cmd.Parameters.AddWithValue("@ClientCategoryDateID", clientCategoryDateID);
            }

            // Route to corresponding stored procedure
            switch (dmlSearchType)
            {
                case "locationNear":
                    {
                        // Call function
                        // SPQueryXML(cmd, "dbo.spGeoPointNRadiusXML")
                        SPQueryXML(ref cmd, "dbo.spGOSGetAssociationDataRadiusXML");
                        break;
                    }

                case "GetFromShapeTypeID":
                    {
                        SPQueryXML(ref cmd, "dbo.spGeoGetFromShapeTypeIDXML");
                        break;
                    }

                case "GetNeiDataShp":
                    {
                        // Use the correct SP depending on requested datatype
                        if ((requestDataType == "Clients"))
                            SPQueryXML(ref cmd, "dbo.spGeoGetNeiDataShpCliXML");
                        else if ((requestDataType == "Projects"))
                            SPQueryXML(ref cmd, "dbo.spGeoGetNeiDataShpProXML");
                        break;
                    }

                case "GeoGetFromCitywideCCDC":
                    {
                        SPQueryXML(ref cmd, "dbo.spGeoGetFromCitywideCCDCXML");
                        break;
                    }

                case "shapesNeighborhoodsList":
                    {
                        // SPQueryJSON(cmd, "dbo.spGeoShpGetNeighborhoodByUserEmailAddr")
                        SPQueryJSON(ref cmd, "dbo.spGeoShpGetNeighborhoodByCity");
                        break;
                    }

                case "shapesCensusTractList":
                    {
                        SPQueryJSON(ref cmd, "dbo.spGeoShpGetCensusTractByUserEmailAddr");
                        break;
                    }

                case "shapesZipCodesByCity":
                    {
                        SPQueryJSON(ref cmd, "dbo.spGeoShpGetZipCodesByCity");
                        break;
                    }

                case "shapesCongressDistricts":
                    {
                        SPQueryXML(ref cmd, "dbo.spGeoShpCongressDistricts");
                        break;
                    }

                case "shapesSupervisors":
                    {
                        SPQueryXML(ref cmd, "dbo.spGeoShpSupervisors");
                        break;
                    }

                case "shapesUserPolygonList":
                    {
                        SPQueryXML(ref cmd, "dbo.spGeoShpGetUserPolygonList");
                        break;
                    }

                case ("GetAgencyAddrByUserEmailAddr"):
                    {
                        SPQueryXML(ref cmd, "dbo.spGOSGetAgencyAddrByUserEmailAddr");
                        break;
                    }

                case "GetSupShapeData":
                    {
                        // Use the correct SP depending on requested datatype
                        if ((requestDataType == "Clients"))
                            SPQueryXML(ref cmd, "dbo.spGeoGetSupDataShpCliXML");
                        else if ((requestDataType == "Projects"))
                            SPQueryXML(ref cmd, "dbo.spGeoGetSupDataShpProXML");
                        break;
                    }

                case "GetUserPolygonShapeData":
                    {
                        // Use the correct SP depending on requested datatype
                        if ((requestDataType == "Clients"))
                            SPQueryXML(ref cmd, "dbo.spGeoShpGetUserPolygonXML");
                        else if ((requestDataType == "Businesses"))
                            SPQueryXML(ref cmd, "dbo.spGeoShpGetUserPolygonXML");
                        break;
                    }

                case "GetUserPolygonShapeIntersectionData":
                    {
                        // Use the correct SP depending on requested datatype
                        if ((requestDataType == "Clients"))
                            SPQueryXML(ref cmd, "dbo.spGeoShpGetUserPolygonIntersectXML");
                        else if ((requestDataType == "Businesses"))
                            SPQueryXML(ref cmd, "dbo.spGeoShpGetUserPolygonIntersectXML");
                        break;
                    }

                case "GetCensusTractData":
                    {
                        if ((requestDataType == "Projects"))
                            SPQueryXML(ref cmd, "dbo.spGeoGetCensusTractDataShpProXML");
                        break;
                    }

                case "GetZipCodesShapeData":
                    {
                        if ((requestDataType == "Property"))
                            SPQueryXML(ref cmd, "dbo.spGeoGetZipCodesDataShpByID");
                        break;
                    }

                case "InsertUserPolygon":
                    {
                        SPQueryXML(ref cmd, "dbo.spGeoShpInsertUserPolygon");
                        break;
                    }

                case "DistClientToAgency":
                    {
                        SPQueryXML(ref cmd, "dbo.spGeoGetDistClientsToAgencyXML");
                        break;
                    }

                case "DistClientToAgencyID":
                    {
                        SPQueryXML(ref cmd, "dbo.spGeoGetDistClientsToAgencyIDXML");
                        break;
                    }

                case "DistClientToAgencyJSON":
                    {
                        SPQueryJSON(ref cmd, "dbo.spGeoGetDistClientsToAgency");
                        break;
                    }

                case "shapesNeighborhoodsJson":
                    {
                        SPQueryJSON(ref cmd, "dbo.spGeoShpNeighborhoodsJson");
                        break;
                    }

                case "ShpSuperDist":
                    {
                        SPQueryJSON(ref cmd, "dbo.spGeoShpSuperDist");
                        break;
                    }

                case "ShpCongDist":
                    {
                        SPQueryJSON(ref cmd, "dbo.spGeoShpCongDist");
                        break;
                    }

                case "ShpStSenateDist":
                    {
                        SPQueryJSON(ref cmd, "dbo.spGeoShpStSenateDist");
                        break;
                    }

                case "ShpStAsmblyDists":
                    {
                        SPQueryJSON(ref cmd, "dbo.spGeoShpStAsmblyDists");
                        break;
                    }

                case "ShpPrecincts":
                    {
                        SPQueryJSON(ref cmd, "dbo.spGeoShpPrecincts");
                        break;
                    }

                case "ShpSFZipCodes":
                    {
                        SPQueryJSON(ref cmd, "dbo.spGeoShpSFZipCodes");
                        break;
                    }

                case "GetClientServicesDatesByClientID":
                    {
                        // SPQueryJSON(cmd, "dbo.spAXGetClientServices")
                        SPQueryJSON(ref cmd, "dbo.spVOLGetClientServicesDatesByClientID");
                        break;
                    }

                case "GetPersonCategory":
                    {
                        SPQueryJSON(ref cmd, "dbo.spGMSGetPersonCategory");
                        break;
                    }

                case "GetProcessTypesByClientCategory":
                    {
                        SPQueryJSON(ref cmd, "dbo.spFSEGetProcessTypesByClientCategory");
                        break;
                    }

                case "GetFamilyMembersByClientID":
                    {
                        SPQueryJSON(ref cmd, "dbo.spVOLGetFamilyMembersByClientID");
                        break;
                    }

                case "RemoveClientCategoryDate":
                    {
                        SPQueryJSON(ref cmd, "dbo.spVOLRemoveClientCategoryDate");
                        break;
                    }

                case "GetClientDataTypes":
                    {
                        SPQueryJSON(ref cmd, "dbo.spGMSGetClientDataTypes");
                        break;
                    }
            }
        }
        // ''' <summary>
        // ''' Search Location Function to call XML sp request
        // ''' </summary>
        // ''' <param name="cmd">SQL Command type that contains required parameters for stored procedure</param>
        // ''' <remarks></remarks>
        // Private Sub SearchLocationsnearXML(ByRef cmd As SqlCommand)
        // SPQueryXML(cmd, "dbo.spGeoPointNRadiusXML")
        // End Sub

        /// <summary>
    /// Generic Query to called stored procedure with parameters and expect a XML result to be returned to the HTTP Response.
    /// </summary>
    /// <param name="cmd">SQL Command type that contains required parameters for stored procedure</param>
    /// <param name="sp">Stored Procedure name to be called</param>
    /// <remarks></remarks>
        private void SPQueryXML(ref SqlCommand cmd, string sp)
        {
            // Here you make the call to your locations stored procedure
            // This database call is a little messy but is just to show you the point.
            // You should really use the MS Application Blocks and/or some other seperate data layer
            var connDB = new SqlConnection();
            // connDB.ConnectionString = "Server=(local);Database=Geo;Trusted_Connection=True;"  ' Enter your own connection string here
            // connDB.ConnectionString = "Server=VOLTRONPC,4938\SQL2008;Initial Catalog=Geo;User ID=sa;Password=admin12;"
            connDB.ConnectionString = connectionString;
            connDB.Open();
            cmd.Connection = connDB;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandTimeout = 20;
            cmd.CommandText = sp;

            // cmd.Parameters.AddWithValue("@dmlLat", 51.49295)
            // cmd.Parameters.AddWithValue("@dmlLng", -0.17728)
            // cmd.Parameters.AddWithValue("@intRadius", 1)

            // Execute the stored procedure and return the result as plain XML
            XmlReader rdrXMLLocations = null;
            // Try
            rdrXMLLocations = cmd.ExecuteXmlReader();
            // Catch ex As Data.SqlClient.SqlException 'SqlException ' Exception ' System.Data.SqlTypes.SqlNullValueException
            // ''Dim contactDoc As XDocument = New XDocument
            // rdrXMLLocations = XmlReader.Create(New IO.StringReader("<markers></markers>"))
            // 'System.Diagnostics.Debug.WriteLine("Caught: SqlNullValue Exception: " + ex.ToString)
            // End Try


            Response.Expires = 0;
            Response.ContentType = "text/xml";
            XmlDocument oDocument = new XmlDocument();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            using (rdrXMLLocations)
            {
                while (!rdrXMLLocations.EOF)
                {
                    rdrXMLLocations.MoveToContent();
                    sb.Append(rdrXMLLocations.ReadOuterXml());
                }
                rdrXMLLocations.Close();
            }
            if (!string.IsNullOrEmpty(sb.ToString()))
                oDocument.LoadXml(sb.ToString());
            oDocument.Save(Response.Output);
            Response.OutputStream.Flush();
            Response.OutputStream.Close();
        }

        /// <summary>
    /// Generic Query to called stored procedure with parameters and expect a XML result to be returned to the HTTP Response.
    /// </summary>
    /// <param name="cmd">SQL Command type that contains required parameters for stored procedure</param>
    /// <param name="sp">Stored Procedure name to be called</param>
    /// <remarks></remarks>
        private void SPQueryJSON(ref SqlCommand cmd, string sp)
        {
            // Here you make the call to your locations stored procedure
            // This database call is a little messy but is just to show you the point.
            // You should really use the MS Application Blocks and/or some other seperate data layer
            var connDB = new SqlConnection();
            // connDB.ConnectionString = "Server=(local);Database=Geo;Trusted_Connection=True;"  ' Enter your own connection string here
            // connDB.ConnectionString = "Server=VOLTRONPC,4938\SQL2008;Initial Catalog=Geo;User ID=sa;Password=admin12;"
            connDB.ConnectionString = connectionString;
            connDB.Open();
            cmd.Connection = connDB;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandTimeout = 20;
            cmd.CommandText = sp;



            DataTable dt = new DataTable();

            try
            {
                SqlDataReader dr = default(SqlDataReader);

                dr = cmd.ExecuteReader();

                dt.Load(dr);

                dr.Close();
            }
            catch (Exception e)
            {
                // MessageBox.Show(e.ToString)
                Console.WriteLine(e.ToString());
            }

            // cmd.Parameters.AddWithValue("@dmlLat", 51.49295)
            // cmd.Parameters.AddWithValue("@dmlLng", -0.17728)
            // cmd.Parameters.AddWithValue("@intRadius", 1)

            // Execute the stored procedure and return the result as plain XML
            // Dim rdrXMLLocations As XmlReader = Nothing
            // Try
            // rdrXMLLocations = cmd.ExecuteXmlReader()
            // Catch ex As Data.SqlClient.SqlException 'SqlException ' Exception ' System.Data.SqlTypes.SqlNullValueException
            // ''Dim contactDoc As XDocument = New XDocument
            // rdrXMLLocations = XmlReader.Create(New IO.StringReader("<markers></markers>"))
            // 'System.Diagnostics.Debug.WriteLine("Caught: SqlNullValue Exception: " + ex.ToString)
            // End Try

            Newtonsoft.Json.JsonSerializerSettings serializerSettings = new Newtonsoft.Json.JsonSerializerSettings();
            serializerSettings.Converters.Add(new DANA.DataTableJsonConverter());
            string jsonData = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None, serializerSettings);

            Response.Expires = 0;
            Response.ContentType = "text/json";
            // Dim oDocument As New XmlDocument()

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            // sb.Append(jsonData)

            sb.Append(JsonConvert.SerializeObject(dt));

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            Response.OutputStream.Write(bytes, 0, bytes.Length);

            // Response.OutputStream.Write(jsonData)
            Response.OutputStream.Flush();
            Response.OutputStream.Close();

            connDB.Close();
            connDB.Dispose();
            SqlConnection.ClearAllPools();
        }
    }
}
