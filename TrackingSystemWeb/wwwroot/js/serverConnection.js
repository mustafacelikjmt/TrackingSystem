import { updateMap } from './maps.js'

export function serverConnection () {
    if (typeof signalR !== 'undefined') {
        var connection = new signalR.HubConnectionBuilder().withAutomaticReconnect([1000, 2000, 3000, 4000, 5000]).withUrl("/LocationHub").build()
        //Functions
        function statusShow () { $("#conStatus").text(connection.state) }
        function start () {
            connection.start().then(() => {
                $("#loading").hide()
                statusShow()
            }).catch((err) => {
                console.log(err)
                setTimeout(() => start(), 2000)
            })
            statusShow()
        }
        statusShow()
        start()
        connection.onreconnecting(err => {
            $("#loading").show()
            statusShow()
        })
        connection.onreconnected(connectionId => {
            $("#loading").hide()
            statusShow()
            console.log("connectionId: " + connectionId)
        })
        connection.onclose(() => {
            $("#loading").hide()
            statusShow()
            start()
        })
        connection.on("ReceiveClientCount", (clientCount) => {
            $("#clientCount").text(clientCount)
        })
        connection.on("ReceiveName", (lokasyon) => {
            //var time = moment(lokasyon.timestamp).format('YYYY-MM-DD T HH:mm:ss')
            //console.log(time)
            updateMap(lokasyon)
        })
    }
}