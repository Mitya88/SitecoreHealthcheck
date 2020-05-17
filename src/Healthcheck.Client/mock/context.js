module.exports = {
  'GET /sitecore/api/ssc/sci/context/-/getcontext': function (req, res) {
    res.json({
      "User": {
        "Profile": {
          "FullName": "Mock Profile Full Name"
        },
        "LocalName": "Mock Local Name"
      },
      "Language": {
        "Name": "en"
      }
    });
  }
};
