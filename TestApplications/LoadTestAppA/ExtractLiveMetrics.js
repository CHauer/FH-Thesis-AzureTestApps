window.setInterval(extractData, 500);

function extractData() {
    var memoryValue = null;
    var cpuValue = null;
    var requestValue = null;
    var requestFailValue = null;
    var timestamp = null;

    var serverData = document.getElementById("serversInfoGrid");

    var requestNodes = serverData.getElementsByClassName("slick-cell l1 r1 slick-number-column");
    var requestFailNodes = serverData.getElementsByClassName("slick-cell l2 r2 slick-number-column");
    var cpuNodes = serverData.getElementsByClassName("slick-cell l3 r3 slick-number-column");
    var memoryNodes = serverData.getElementsByClassName("slick-cell l4 r4 slick-number-column");

    //console.log(cpuNodes.length);

    if (cpuNodes != null && cpuNodes.length >= 1 && requestNodes != null && requestNodes.length >= 1
        && requestFailNodes != null && requestFailNodes.length >= 1 && memoryNodes != null && memoryNodes.length >= 1) {
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
            timestamp: timestamp,
            cpu: cpuValue,
            memory: memoryValue,
            requests: requestValue,
            requestsFailed: requestFailValue
        });
    }
}