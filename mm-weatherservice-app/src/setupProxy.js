const { createProxyMiddleware } = require('http-proxy-middleware');

const context = [
    "/weather",
];

module.exports = function (app) {
    const appProxy = createProxyMiddleware(context, {
        target: 'https://localhost:7033',
        secure: false,
        onProxyReq: (proxyReq, req, res) => {
            console.log('proxyReq called');
            proxyReq.setHeader('x-api-key', 'API-dummy.key.1');
        }
    });

    app.use(appProxy);
};
