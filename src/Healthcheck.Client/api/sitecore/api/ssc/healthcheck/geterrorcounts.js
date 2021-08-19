module.exports = (req, res) => {
  const { name = 'World' } = req.query;
  res.status(200).json({"Dates":["2020-12-17","2020-12-16","2020-12-15"],"Counts":[297,240,50]});
};