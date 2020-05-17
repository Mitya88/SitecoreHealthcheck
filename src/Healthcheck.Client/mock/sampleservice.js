module.exports = {
  'GET /sitecore/api/ssc/sample/service': function (req, res) {
    res.json({
      "Sample":{"Title":"Sample Service", "Description":"This is the description of sample service"}
    });
  },
  'GET /sitecore/api/ssc/sample/run': function (req, res) {
    res.json({
      "Sample":{"Title":"Sample Service", "Description":"This is the description of sample service"}
    });
  },
  'GET /sitecore/api/ssc/sample/service': function (req, res) {
    res.json({
      "Sample":{"Title":"Sample Service", "Description":"This is the description of sample service"}
    });
  }
};
