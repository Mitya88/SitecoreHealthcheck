module.exports = {
  'GET /sitecore/api/ssc/sci/access/-/checkaccess': function (req, res) {
    res.json([
      { "itemName": "OtherFieldDisabledToDevs", "access": { "read": true, "write": false } },
      { "itemName": "SomeFieldDisabledToDevs", "access": { "read": true, "write": false } },
      { "itemName": "SomeInfoHiddenToDevs", "access": { "read": false, "write": false } }
    ]);
  }
};
