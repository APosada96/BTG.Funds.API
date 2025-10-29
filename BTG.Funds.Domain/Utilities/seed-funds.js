// Seleccionar la base de datos
db = db.getSiblingDB("BTGFundsDB");

// Crear colección de fondos si no existe
if (!db.getCollectionNames().includes("Funds")) {
    db.createCollection("Funds");
}

// Insertar fondos iniciales
db.Funds.insertMany([
    { Name: "FPV_BTG_PACTUAL_RECAUDADORA", MinimumAmount: 75000, Category: "FPV" },
    { Name: "FPV_BTG_PACTUAL_ECOPETROL", MinimumAmount: 125000, Category: "FPV" },
    { Name: "DEUDAPRIVADA", MinimumAmount: 50000, Category: "FIC" },
    { Name: "FDO-ACCIONES", MinimumAmount: 250000, Category: "FIC" },
    { Name: "FPV_BTG_PACTUAL_DINAMICA", MinimumAmount: 100000, Category: "FPV" }
]);

// Crear usuario base con saldo inicial
if (!db.getCollectionNames().includes("UserAccounts")) {
    db.createCollection("UserAccounts");
}

db.UserAccounts.insertOne({
    Balance: 500000,
    SubscribedFunds: []
});

print("✅ Fondos y usuario inicial insertados correctamente en BTGFundsDB");
