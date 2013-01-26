<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TokenBasedAccessPlugin.ascx.cs" Inherits="POSSIBLE.TokenBasedAccess.UI.TokenBasedAccessPlugin" %>
<%@ Register Namespace="EPiServer.UI.WebControls" Assembly="EPiServer.UI" TagPrefix="EPiServerUI" %>    
<%@ Register TagPrefix="EPiServer" Namespace="EPiServer.Web.WebControls" Assembly="EPiServer" %>   

<style type="text/css">
    td.expired { color: #ccc;}    
</style>

<script text="text/javascript">
    window.onload = function () {
        var labelId = '<%=limitLabel.ClientID %>';
        var dropdownId = '<%=ddlEnquiryTypes.ClientID %>';
        var labelElement = document.getElementById(labelId);
        var dropdownElement = document.getElementById(dropdownId);

        dropdownElement.onchange = function() {
            if (dropdownElement.selectedIndex === 1) {
                labelElement.innerHTML = '<%=Translate("/tokenbasedaccess/datelimit")%>';
            } else {
                labelElement.innerHTML = '<%=Translate("/tokenbasedaccess/usagelimit")%>';
            }
        };
    }
</script>

<div class="epi-contentContainer epi-padding">
    <div class="epi-contentArea">
        <h1 class="epi-prefix"><%=Translate("/tokenbasedaccess/title")%></h1>
        <p class="EP-systemInfo"><%=Translate("/tokenbasedaccess/description")%></p>

        <div class="epi-formArea"> 
            <fieldset>
                <legend><%=Translate("/tokenbasedaccess/createtoken")%></legend> 
            
                <div class="epi-size10">                        
                    <div>
                        <asp:Label ID="Label1" AssociatedControlID="ddlEnquiryTypes" runat="server"><%=Translate("/tokenbasedaccess/expirytype")%></asp:Label>
                        <asp:DropDownList ID="ddlEnquiryTypes" runat="server" />
                    </div>
                </div>
            
                <div class="epi-size10" id="limitContainerElement">                        
                    <div>
                        <asp:Label ID="limitLabel" AssociatedControlID="txtLimit" runat="server"><%=Translate("/tokenbasedaccess/usagelimit")%></asp:Label>
                        <asp:TextBox ID="txtLimit" runat="server" />
                        <asp:CompareValidator runat="server" Display="Dynamic" ControlToValidate="txtLimit" Type="Integer" Operator="DataTypeCheck" ErrorMessage="<%$ Resources: EPiServer, tokenbasedaccess.limitvalidationerror %>" />
                        <asp:RequiredFieldValidator runat="server" Display="Dynamic" ControlToValidate="txtLimit" ErrorMessage="<%$ Resources: EPiServer, tokenbasedaccess.limitvalidationerror %>"/>
                    </div>
                </div>
                
                <div class="epi-size10">                        
                    <div>
                        <asp:Label ID="Label3" AssociatedControlID="txtEmail" runat="server"><%=Translate("/tokenbasedaccess/addsendto")%></asp:Label>
                        <asp:TextBox ID="txtEmail" runat="server" />
                        <asp:RegularExpressionValidator Display="Dynamic" ControlToValidate="txtEmail" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ErrorMessage="<%$ Resources: EPiServer, tokenbasedaccess.emailvalidationerror %>" runat="server" />
                        <asp:RequiredFieldValidator runat="server"  Display="Dynamic" ControlToValidate="txtEmail" ErrorMessage="<%$ Resources: EPiServer, tokenbasedaccess.emailvalidationerror %>"/>
                    </div>
                </div>
             
                <div class="epi-buttonDefault">
                    <EPiServerUI:ToolButton id="AddRowButton" OnClick="AddNewToken_OnClick" runat="server" SkinID="Add" text="<%$ Resources: EPiServer, button.add %>" ToolTip="<%$ Resources: EPiServer, button.add %>" />
                </div>
            </fieldset>
            
            <table class="epi-default" cellspacing="0" border="0" style="border-style:None;border-collapse:collapse;">
                <tbody>
	                <tr>
	                    <th><%=Translate("/tokenbasedaccess/sentto")%></th>
                        <th><%=Translate("/tokenbasedaccess/created")%></th>
                        <th><%=Translate("/tokenbasedaccess/expiry")%></th> 
                        <th>&nbsp;</th>
	                </tr>

                    <asp:Repeater ID="rptPageTokens" runat="server" OnItemDataBound="rptPageTokens_OnItemDataBound">
                        <ItemTemplate>
                            <tr>
                               <td class='<asp:Literal ID="td1" runat="server" />'><%# Eval("SentTo") %></td>
                               <td class='<asp:Literal ID="td2" runat="server" />'><%# (((DateTime)Eval("Created")).ToLongDateString()) %></td>
                               <td class='<asp:Literal ID="td3" runat="server" />'>
                                   <asp:Literal runat="server" ID="Literal1" />
                               </td>
                               <td class='<asp:Literal ID="td4" runat="server" />' style="text-align: center;">
                                    <EPiServerUI:ToolButton id="ToolButton1" OnClick="ResendToken_OnClick" runat="server" SkinID="redo" CausesValidation="False" ToolTip="<%$ Resources: EPiServer, tokenbasedaccess.resend %>" />
                                    <EPiServerUI:ToolButton id="ToolButton2" OnClick="DeleteToken_OnClick" runat="server" SkinID="Delete" CausesValidation="False" ToolTip="<%$ Resources: EPiServer, button.delete %>" />
                               </td>
                           </tr>    
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table> 
        </div>
    </div>
</div>