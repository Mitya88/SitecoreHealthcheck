module.exports = (req, res) => {
  const { name = 'World' } = req.query;
  res.status(200).json([{"Name":"debug","Size":"0 bytes"},{"Name":"DeviceDetection","Size":"484.5 MB"},{"Name":"diagnostics","Size":"24.1 MB"},{"Name":"logs","Size":"7.7 MB"},{"Name":"MediaCache","Size":"121.3 KB"},{"Name":"packages","Size":"5.9 MB"},{"Name":"Submit Queue","Size":"0 bytes"},{"Name":"tools","Size":"17.7 MB"},{"Name":"viewstate","Size":"258.8 KB"}]);
};