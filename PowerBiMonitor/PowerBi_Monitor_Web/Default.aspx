<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PbiMonitor_Web.Default" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>

    <script type="text/javascript" src="scripts/powerbi.js"></script>

    <script type="text/javascript">

        //This code is for sample purposes only.

        //Configure IFrame for the Report after you have an Access Token. See Default.aspx.cs to learn how to get an Access Token
        window.onload = function () {
            var accessToken = document.getElementById('MainContent_accessToken').value;

            if (!accessToken || accessToken == "")
            {
                return;
            }

            var embedUrl = document.getElementById('MainContent_hidEmbedUrl').value;
            var reportId = document.getElementById('MainContent_hidReportId').value;

            // Embed configuration used to describe the what and how to embed.
            // This object is used when calling powerbi.embed.
            // This also includes settings and options such as filters.
            // You can find more information at https://github.com/Microsoft/PowerBI-JavaScript/wiki/Embed-Configuration-Details.
            var config= {
                type: 'report',
                accessToken: accessToken,
                embedUrl: embedUrl,
                id: reportId,
                settings: {
                    filterPaneEnabled: true,
                    navContentPaneEnabled: true
                }
            };

            // Grab the reference to the div HTML element that will host the report.
            var reportContainer = document.getElementById('reportContainer');

            // Embed the report and display it within the div container.
            var report = powerbi.embed(reportContainer, config);

            // Report.on will add an event handler which prints to Log window.
            report.on("loaded", function () {
                var logView = document.getElementById('logView');
                logView.innerHTML = logView.innerHTML + "Loaded<br/>";

                // Report.off removes a given event handler if it exists.
                report.off("loaded");
            });

            // Report.on will add an event handler which prints to Log window.
            report.on("rendered", function () {
                var logView = document.getElementById('logView');
                logView.innerHTML = logView.innerHTML + "Rendered<br/>";

                // Report.off removes a given event handler if it exists.
                report.off("rendered");
            });
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Get a reference to the embedded report HTML element
            //var embedContainer1 = $('#reportContainer')[0];

            // Get a reference to the embedded report.
            //BaseTemplate = powerbi.get(embedContainer);

            // Report.off removes a given event listener if it exists.
            report.off("dataSelected");

            // Report.on will add an event listener.
            report.on("dataSelected", function (event) {
                var data = event.detail;
                //Log.log(data);
                logView.innerHTML = JSON.stringify(data,null, 4);
                //alert(data.dataPoints[0].identity[10].equals);

                var modaltext = document.getElementById('ModalText');
                //modaltext.innerText = data.dataPoints[0].identity[10].equals;
                modaltext.innerText = JSON.stringify(data, null, "\t");
                var modal = document.getElementById('myModal');
                // Get the <span> element that closes the modal
                var span = document.getElementsByClassName("close")[0];
                modal.style.display = "block";
           
                span.onclick = function () {
                    modal.style.display = "none";
                }

            });

    


        };
    </script>

    <form id="form1" runat="server">

    <asp:HiddenField ID="accessToken" runat="server" />

    <header>
        <h1>
            Power BI Embed Report
        </h1>
    
    </header>
    <asp:HiddenField ID="hidReportId" runat="server" />
    
     <asp:HiddenField ID="hidEmbedUrl" runat="server" />
    <div class="field">
         <div class="fieldtxt">Report Name</div>
        <asp:Textbox ID="txtReportName" runat="server" Width="750px">CostingToolReport</asp:Textbox>
    </div>
   <div>
        <h3>
            Select <b>"Get Report"</b> to get and embed first report from your Power BI account.
        </h3>
        <asp:Button ID="getReportButton" runat="server" OnClick="getReportButton_Click" Text="Find And Embed reprt" />  
    </div>
   

    <div>
        Embedded Report
        <br />
        <div ID="reportContainer" style="width: 900px; height: 500px"></div>
    </div>

    <div>
        Log View
        <br />
        <div ID="logView" style="width: 880px;"></div>
    </div>

<div id="myModal" class="modal">

  <!-- Modal content -->
  <div class="modal-content">
    <span class="close">&times;</span>
    <p id="ModalText">..</p>
  </div>

</div>
</form>
</body>
</html>
