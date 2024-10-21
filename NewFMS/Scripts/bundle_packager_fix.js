// In react-native/packager/debugger, replace the 'executeApplicationScript' function with this code:

'executeApplicationScript'=== function(message, sendReply) {
  for (var key in message.inject) {
    window[key] = JSON.parse(message.inject[key]);
  }

  if(message.url.split(':')[0] === 'file'){
    var newUrl = 'http://localhost:8081/' + message.url;
    loadScript(newUrl, sendReply.bind(null, null));
  } else {
    loadScript(message.url, sendReply.bind(null, null));
  }
},

// In packager.js, replace the getDevToolsLauncher function with this:

function getDevToolsLauncher(options) {
  return function(req, res, next) {
    if (req.url === '/debugger-ui') {
      var debuggerPath = path.join(__dirname, 'debugger.html');
      res.writeHead(200, {'Content-Type': 'text/html'});
      fs.createReadStream(debuggerPath).pipe(res);
    } else if (req.url === '/launch-chrome-devtools') {
      var debuggerURL = 'http://localhost:' + options.port + '/debugger-ui';
      var script = 'launchChromeDevTools.applescript';
      console.log('Launching Dev Tools...');
      execFile(path.join(__dirname, script), [debuggerURL], function(err, stdout, stderr) {
        if (err) {
          console.log('Failed to run ' + script, err);
        }
        console.log(stdout);
        console.warn(stderr);
      });
      res.end('OK');
    } else if(req.url.match(new RegExp('file://'))) {
      fs.readFile(req.url.replace(new RegExp('/file://'), ''), function (err, data) {
        if(err) console.log(err);
        res.end(data);
      })
    } else {
      next();
    }
  };
}
