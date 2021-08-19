module.exports = (req, res) => {
  const { name = 'World' } = req.query;
  res.status(200).json({"IsAdministrator":true, "HealthyCount":10,"WarningCount":3,"ErrorCount":2,"UnknownCount":1});
};