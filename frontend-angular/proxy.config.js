// https://angular.io/guide/build#proxying-to-a-backend-server

const PROXY_CONFIG = {
  '/users/**': {
    target: 'https://api.github.com',
    changeOrigin: true,
    secure: false,
    logLevel: 'debug',
    // onProxyReq: (proxyReq, req, res) => {
    //   const cookieMap = {
    //     SID: '',
    //   };
    //   let cookie = '';
    //   for (const key of Object.keys(cookieMap)) {
    //     cookie += `${key}=${cookieMap[key]}; `;
    //   }
    //   proxyReq.setHeader('cookie', cookie);
    // },
  },
  '/api': {
    target: 'http://localhost:5198',
    secure: false,
    changeOrigin: true,
    pathRewrite: {
      '^/api': '/api/v1',
    },
    logLevel: 'debug',
  },
};

module.exports = PROXY_CONFIG;
