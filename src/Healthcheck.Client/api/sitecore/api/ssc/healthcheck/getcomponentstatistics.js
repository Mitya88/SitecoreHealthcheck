module.exports = (req, res) => {
  const { name = 'World' } = req.query;
  res.status(200).json({"HealthyCount":10,"WarningCount":9,"ErrorCount":2,"UnknownCount":1});
};