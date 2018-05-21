window.setInterval(extractData, 500);

function extractData() {
    getServerData("even");
    getServerData("odd");
}

function getServerData(serverrow) {
    var memoryValue = null;
    var cpuValue = null;
    var requestValue = null;
    var requestFailValue = null;
    var timestamp = null;
    var server = null;

    var serverData = document.getElementById("serversInfoGrid");
    var serverInstances = serverData.getElementsByClassName("ui-widget-content slick-row " + serverrow);

    if(serverInstances == null || serverInstances.length <= 0){
        return;
    }

    var serverInstance = serverInstances[0];

    var serverNodes  = serverInstance.getElementsByClassName("slick-cell l0 r0");
    var requestNodes = serverInstance.getElementsByClassName("slick-cell l1 r1 slick-number-column");
    var requestFailNodes = serverInstance.getElementsByClassName("slick-cell l2 r2 slick-number-column");
    var cpuNodes = serverInstance.getElementsByClassName("slick-cell l3 r3 slick-number-column");
    var memoryNodes = serverInstance.getElementsByClassName("slick-cell l4 r4 slick-number-column");

    //console.log(cpuNodes.length);

    if (cpuNodes != null && cpuNodes.length >= 1 &&
        requestNodes != null && requestNodes.length >= 1 && 
        requestFailNodes != null && requestFailNodes.length >= 1 &&
        memoryNodes != null && memoryNodes.length >= 1 && 
        serverNodes != null && serverNodes.length >= 1) {

        server = serverNodes[0].innerHTML;
        cpuValue = cpuNodes[0].innerHTML;
        requestValue = requestNodes[0].innerHTML;
        requestFailValue = requestFailNodes[0].innerHTML;
        memoryValue = memoryNodes[0].innerHTML;

        timestamp = new Date().getTime()

        //console.info(timestamp +
        //    "- cpu: " + cpuValue + "/ memory: " + memoryValue +
        //    "/ request/sec: " + requestValue + "/ request failed/sec: " + requestFailValue);

        //var xhr = new XMLHttpRequest();
        //xhr.open("POST", "http://localhost:11405/metric", true);
        //xhr.send({
        //    timestamp: timestamp,
        //    cpu: cpuValue,
        //    memory: memoryValue,
        //    requests: requestValue,
        //    requestsFailed: requestFailValue
        //});

        console.log({
            server: server,
            time: timestamp,
            cpu: cpuValue,
            ram: memoryValue,
            req: requestValue,
            //reqFail: requestFailValue
        });
    }
}