// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:5001/ServerHub", { accessTokenFactory: () => "token" })
    .configureLogging(signalR.LogLevel.Information)
    .build();

async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

connection.on("paymentsUpdate", (payment) => {
    document.getElementById("text").value += "Pay ID:" + payment.paymentId + "\n" + "Pay Status:" + payment.status + "\n" + "Pay value:" + payment
        .paid + "\n"
});

connection.onclose(async () => {
    await start();
});

// Start the connection.
start();

async function SendPay() {
    var idpay = document.getElementById('idpay').value
    try {
        var obj = {
            PaymentId: parseInt(idpay),
            Status: "Completed",
            Paid: Math.random() * (1000 - 1) + 1
        };
        await connection.invoke("PaymentsUpdate", obj);
    } catch (err) {
        console.error(err);
    }
}