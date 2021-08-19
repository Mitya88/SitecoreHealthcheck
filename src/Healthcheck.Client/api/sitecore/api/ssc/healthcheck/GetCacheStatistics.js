module.exports = (req, res) => {
  const { name = 'World' } = req.query;
  res.status(200).json({"FullCacheSize":3179835492,"UsedCacheSize":81951656,"FullCacheSizeText":"3 GB","UsedCacheText":"78.2 MB"});
};