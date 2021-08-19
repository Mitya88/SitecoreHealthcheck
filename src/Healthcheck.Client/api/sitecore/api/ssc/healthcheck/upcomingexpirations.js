module.exports = (req, res) => {
  const { name = 'World' } = req.query;
  res.status(200).json([{"Name":"Sitecore Licence","Type":"Licence","Date":"2021-01-07T12:55:46.9473695+01:00"}]);
};