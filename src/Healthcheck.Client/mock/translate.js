module.exports = {
  'GET /sitecore/api/ssc/sci/translate/B76C8EC2-1139-4BB1-915D-0F0DB4A04FE4/gettranslations': function (req, res) {
    res.json([]);
  },
  'GET /sitecore/api/ssc/sci/translate/2218B78E-5A07-462F-A9D6-15E086742612/gettranslations': function (req, res) {
    res.json([
      { Phrase: 'Back', Key: 'BACK' },
      { Phrase: 'Log out', Key: 'LOG_OUT' }
    ]);
  }
};
