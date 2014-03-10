
csapp.filter('custBillSubString', function() {
    return function(columnNames) {
        var filtered = [];
        angular.forEach(columnNames, function(columnName) {
            if (columnName.search("CustBillViewModel") > -1) {
                var cust = columnName.replace("CustBillViewModel", "Customer");
                filtered.push(cust);
            }
            if (columnName.search("GPincode") > -1) {
                var pin = columnName.replace("GPincode", "Pincode");
                filtered.push(pin);
            }
            if (columnName.search("StkhBillViewModel") > -1) {
                var stake = columnName.replace("StkhBillViewModel", "Stakeholder");
                filtered.push(stake);
            }
        });
        return filtered;
    };
});

csapp.filter('custBillSubString2', function() {
    return function(columnName) {
        if (columnName.search("CustBillViewModel") > -1) {
            return columnName.replace("CustBillViewModel", "Customer");

        }
        if (columnName.search("GPincode") > -1) {
            return columnName.replace("GPincode", "Pincode");
        }
        if (columnName.search("StkhBillViewModel") > -1) {
            return columnName.replace("StkhBillViewModel", "Stakeholder");
        }
        return columnName;
    };
});
