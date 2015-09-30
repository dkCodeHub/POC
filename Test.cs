using AliteUtils;
using MCIAdapterWebApp.Helpers;
using MCIAdapterWebApp.WebForms.Helpers;
using MCIDAO;
using MCIModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCIAdapterWebApp.Pages
{
    public partial class ChannelManagement : System.Web.UI.Page
    {
        string message = string.Empty;
        string type = string.Empty;
        Type cstype = null;
        AppMessage Message = new AppMessage();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lstChannelParamsDetails = null;
                BindParamToChannel();
                BindParamsDropDown();
                BindGrid();
            }
            cstype = this.GetType();

        }

        public List<ChannelParamsDetails> lstChannelParamsDetails
        {
            get
            {
                if (Session[AppLevelConstants._ChannelParamsDetailsDetails] != null)
                    return Session[AppLevelConstants._ChannelParamsDetailsDetails] as List<ChannelParamsDetails>;
                else
                    return null;
            }
            set
            {
                Session[AppLevelConstants._ChannelParamsDetailsDetails] = value;
            }
        }


        #region DropDown Methods

        private void BindParamsDropDown()
        {
            //TODO : This method will be called on Page Load
            ChannelManegmentDAO _ObjCMDAO = new ChannelManegmentDAO();
            DropDownChannelParams ObjDropDown = _ObjCMDAO.GetChannelParams();
            drpParams.ClearSelection();
            drpParams.Items.Clear();
            drpParams.Attributes.Clear();
            foreach (var item in ObjDropDown.lstChannelParams)
            {
                ListItem _objItem = new ListItem();
                _objItem.Attributes.Add("Description", item.Description);
                _objItem.Attributes.Add("IsRequired", item.IsRequired.ToString());
                _objItem.Text = item.Text;
                _objItem.Value = item.Value.ToString();
                drpParams.Items.Add(_objItem);
            }


            drpParams.DataBind();
        }


        #endregion DropDown Methods


        #region SelectedIndexChanged

        protected void drpParams_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //TODO: This will be called when drpParams is selected. We have to make sure a parameter is defined only once per Channel.
                if (drpParams.SelectedValue != "0" && !string.IsNullOrEmpty(drpParams.SelectedItem.Attributes["Description"].Trim()))
                {
                    hlpDescription.Attributes.Clear();
                    hlpDescription.Attributes.Add("Title", drpParams.SelectedItem.Attributes["Description"].Trim());
                }

                string strIsRequired = drpParams.SelectedItem.Attributes["IsRequired"].Trim();
                if (strIsRequired == "1")
                {
                    RFVParamValue.Enabled = true;
                    string strErrorMessage = "Please provide value for parameter : " + drpParams.SelectedItem.Text;
                    RFVParamValue.ErrorMessage = "<img src='../assets/img/actions/error.png' title='" + strErrorMessage + "' />";
                }
                else
                {
                    RFVParamValue.Enabled = false;
                }
                BindParamToChannel();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        #endregion SelectedIndexChanged


        #region ParameterGridMethods

        private void BindParamToChannel()
        {
            //TODO: This will be used to bind the grid with the parameters along with the value for the mentioned channel.
            if (lstChannelParamsDetails != null && lstChannelParamsDetails.Count > 0)
            {
                grdParamDefinition.DataSource = lstChannelParamsDetails;
                grdParamDefinition.DataBind();
            }
            else
            {
                List<ChannelParamsDetails> BlankList = new List<ChannelParamsDetails>();
                BlankList.Add(new ChannelParamsDetails());
                grdParamDefinition.DataSource = BlankList;
                grdParamDefinition.DataBind();

                int count = grdParamDefinition.Rows[0].Cells.Count;
                grdParamDefinition.Rows[0].Cells.Clear();
                grdParamDefinition.Rows[0].Cells.Add(new TableCell());
                grdParamDefinition.Rows[0].Cells[0].ColumnSpan = count;
                grdParamDefinition.Rows[0].Cells[0].Text = "No Parameters Added.";
                grdParamDefinition.Rows[0].Cells[0].Attributes.Add("style", "padding-left:2px;");
            }
        }

        public void AddParams()
        {
            try
            {
               
                if (Convert.ToInt32(drpParams.SelectedValue) > 0)
                {

                    List<ChannelParamsDetails> _ObjCPD;
                    ChannelParamsDetails ObjModel;

                    int intPrevParamID = hdnPrevParamID.Value == "" ? 0 : Convert.ToInt32(hdnPrevParamID.Value);

                    _ObjCPD = lstChannelParamsDetails as List<ChannelParamsDetails>;

                    int iRowCount = 0;
                    if (_ObjCPD != null && _ObjCPD.Count > 0)
                    {
                        if (intPrevParamID <= 0)
                            iRowCount = _ObjCPD.Where(x => x.ParameterID == Convert.ToInt32(drpParams.SelectedValue)).ToList().Count;
                        else
                            iRowCount = _ObjCPD.Where(x => x.ParameterID == Convert.ToInt32(drpParams.SelectedValue)
                                && (x.ParameterID != intPrevParamID)).ToList().Count;
                    }
                    if (iRowCount <= 0)
                    {
                        if (intPrevParamID <= 0)
                        {
                            if (_ObjCPD == null)
                            {
                                _ObjCPD = new List<ChannelParamsDetails>();
                            }

                            ObjModel = new ChannelParamsDetails();
                            ObjModel.ParameterID = Convert.ToInt32(drpParams.SelectedValue);
                            ObjModel.ParameterName = drpParams.SelectedItem.Text.Trim();
                            ObjModel.ParameterValue = txtParamValue.Text.Trim();
                            _ObjCPD.Add(ObjModel);
                        }
                        else
                        {
                            ObjModel = new ChannelParamsDetails();
                            ObjModel = _ObjCPD.Find(x => x.ParameterID == intPrevParamID);
                            ObjModel.ParameterID = Convert.ToInt32(drpParams.SelectedValue);
                            ObjModel.ParameterName = drpParams.SelectedItem.Text.Trim();
                            ObjModel.ParameterValue = txtParamValue.Text.Trim();

                        }
                        lstChannelParamsDetails = _ObjCPD;
                    }
                    else
                    {
                        //  display error message
                    }

                    ClearGrid();
                    
                }
                else
                {
                    message = "Please select valid Parameter.";
                    type = "info";
                    //Message.Notify(ref this.Page, cstype, "NullParamDef", message, "", type, "", 0, false);
                    //BindParamToChannel();
                    //return;
                    //RFVParamValue.Enabled = true;
                    //string strErrorMessage = "Please provide value for parameter : " + drpParams.SelectedItem.Text;
                    //RFVParamValue.ErrorMessage = "<img src='../assets/img/actions/error.png' title='" + strErrorMessage + "' />";

                    PageLevelMessage("InvalidParam");
                }
                
                
                
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void PageLevelMessage(string csname1)
        {
            string strTostr = string.Empty;
            string shortCust = type;

            strTostr = "toastr['" + shortCust + "']('" + message + "')";


            StringBuilder sb = new StringBuilder();

            sb.Append("toastr.options = {");
            sb.Append("closeButton: true,");
            sb.Append("debug: false,");
            sb.Append("newestOnTop: false,");
            sb.Append("progressBar: false,");
            sb.Append("positionClass: 'toast-center',");
            sb.Append("preventDuplicates: true,");
            sb.Append("onclick: null,");
            sb.Append("};");
            //sb.Append("toastr.options.closeButton = true;");
            //sb.Append("toastr.options.debug = false;");
            //sb.Append("toastr.options.newestOnTop = false;");
            //sb.Append("toastr.options.progressBar = false;");
            //sb.Append("toastr.options.positionClass = 'toast-center';");
            //sb.Append("toastr.options.preventDuplicates = true;");
            //sb.Append("toastr.options.onclick = null;");
            //sb.Append("toastr.options.showDuration = 300;");
            //sb.Append("toastr.options.hideDuration = 1000;");
            //sb.Append("toastr.options.timeOut = 5000;");
            //sb.Append("toastr.options.extendedTimeOut = 1000;");
            //sb.Append("toastr.options.showEasing = 'swing';");
            //sb.Append("toastr.options.hideEasing = 'linear';");
            //sb.Append("toastr.options.showMethod = 'fadeIn';");
            //sb.Append("toastr.options.hideMethod = 'fadeOut';");
            sb.Append(strTostr);

            ClientScriptManager cs = Page.ClientScript;

            if (!cs.IsStartupScriptRegistered(cstype, csname1))
            {
                StringBuilder cstext1 = new StringBuilder();
                cstext1.Append("<script type=text/javascript>");
                cstext1.Append(sb.ToString());
                cstext1.Append("</");
                cstext1.Append("script>");

                cs.RegisterStartupScript(cstype, csname1, cstext1.ToString());

            }
        }

        private void ClearGrid()
        {
            try
            {
                BindParamToChannel();
                drpParams.SelectedValue = "-1";
                txtParamValue.Text = "";
                hdnPrevParamID.Value = "";
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        protected void grdParamDefinition_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                GridView gdvParam = (GridView)sender;
                int iRow = e.NewEditIndex;
                string strParameterID = gdvParam.DataKeys[iRow].Values["ParameterID"].ToString();
                string strParameterValue = gdvParam.DataKeys[iRow].Values["ParameterValue"].ToString();

                drpParams.SelectedValue = strParameterID;
                hdnPrevParamID.Value = strParameterID;
                txtParamValue.Text = strParameterValue;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        protected void grdParamDefinition_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                GridView gdvParam = (GridView)sender;
                int iRow = e.RowIndex;
                string strParameterID = gdvParam.DataKeys[iRow].Values["ParameterID"].ToString();
                string strParameterValue = gdvParam.DataKeys[iRow].Values["ParameterValue"].ToString();

                List<ChannelParamsDetails> _objCPD;
                _objCPD = lstChannelParamsDetails as List<ChannelParamsDetails>;
                _objCPD.RemoveAll(x => x.ParameterID == Convert.ToInt32(strParameterID));
                lstChannelParamsDetails = _objCPD;
                ClearGrid();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        protected void imgAddParams_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                //TODO: This will be called when defining / binding params to the channel.                
                AddParams();
                RFVParamValue.Enabled = false;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        #endregion ParameterGridMethods


        #region ClickEvents

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string message = string.Empty;
            string type = string.Empty;
            Type cstype = this.GetType();

            try
            {
                //string strKey = "popup";
                //string strScript = "RaisePopUp();";
                //ScriptManager.RegisterStartupScript(this, this.GetType(), strKey, strScript, true);

                List<ChannelParamsDetails> LstParams = lstChannelParamsDetails;
                List<ChannelManagementModel> LstHdrModel = new List<ChannelManagementModel>();

                if (LstParams == null || LstParams.Count <= 0)
                {
                    BindParamToChannel();
                    message = "No channel information provided for saving.";
                    type = "error";
                   //////////// AppMessage.Notify(this, cstype, "NullParamDef", message, "", type, "", 0, false);
                    return;
                }

                ChannelManagementModel obj = new ChannelManagementModel();
                obj.ChannelCode = Convert.ToInt32(string.IsNullOrEmpty(hdnChannelCode.Value) ? "0" : hdnChannelCode.Value);
                obj.ChannelType = Convert.ToInt32(drpChannelType.SelectedValue);
                obj.ChannelTypeName = drpChannelType.SelectedItem.Text.Trim();
                obj.ChannelID = txtChannelID.Text.Trim();
                obj.ChannelDescription = txtChannelDesc.Text.Trim();
                obj.lstParamsDetails = LstParams;
                LstHdrModel.Add(obj);

                ChannelManegmentDAO _ObjCMDAO = new ChannelManegmentDAO();
                if (_ObjCMDAO.Save(LstHdrModel))
                {
                    Reset();
                    BindGrid();
                    message = "Data Saved Successfully.";
                    type = "success";
                   ///////// AppMessage.Notify(this, cstype, "ParamSaveSuccess", message, "", type, "", 0, false);
                }
                else
                {
                    message = "Error while saving data.";
                    type = "error";
                   //////// AppMessage.Notify(this, cstype, "ParamSaveError", message, "", type, "", 0, false);
                }

                

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                Reset();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        #endregion ClickEvents


        #region HeaderGrid Methods

        public void Reset()
        {
            try
            {
                txtChannelID.Text = "";
                txtChannelDesc.Text = "";
                hdnChannelCode.Value = "";
                lstChannelParamsDetails = null;
                BindParamToChannel();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void BindGrid()
        {
            ChannelManegmentDAO _ObjCMDAO = new ChannelManegmentDAO();
            gdvParameter.DataSource = _ObjCMDAO.GetChannelData();
            gdvParameter.DataBind();
        }

        protected void gdvParameter_RowEditing(object sender, GridViewEditEventArgs e)
        {
            try
            {
                GridView gdv = (GridView)sender;
                int iRow = e.NewEditIndex;
                string strChannelType = gdv.DataKeys[iRow].Values["ChannelType"].ToString();
                string strChannelID = gdv.DataKeys[iRow].Values["ChannelID"].ToString();
                int intChannelCode = gdv.DataKeys[iRow].Values["ChannelCode"] == null ? 0 : Convert.ToInt32(gdv.DataKeys[iRow].Values["ChannelCode"]);
                string strChannelDescription = gdv.DataKeys[iRow].Values["ChannelDescription"].ToString();

                drpChannelType.SelectedValue = strChannelType;
                txtChannelID.Text = strChannelID;
                hdnChannelCode.Value = intChannelCode.ToString();
                txtChannelDesc.Text = strChannelDescription;

                ChannelManegmentDAO _ObjDAO = new ChannelManegmentDAO();
                lstChannelParamsDetails = _ObjDAO.GetParamDetails(intChannelCode, strChannelID);
                BindParamToChannel();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        protected void gdvParameter_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                GridView gdv = (GridView)sender;
                int iRow = e.RowIndex;
                int intChannelCode = gdv.DataKeys[iRow].Values["ChannelCode"] == null ? 0 : Convert.ToInt32(gdv.DataKeys[iRow].Values["ChannelCode"]);
                string strChannelID = gdv.DataKeys[iRow].Values["ChannelID"] == null ? "" : gdv.DataKeys[iRow].Values["ChannelID"].ToString();

                ChannelManegmentDAO _ObjDAO = new ChannelManegmentDAO();
                _ObjDAO.DeleteChannel(intChannelCode, strChannelID);
                BindGrid();
                lstChannelParamsDetails = null;
                BindParamToChannel();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        #endregion HeaderGrid Methods

    }
}
