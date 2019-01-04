/// <binding BeforeBuild='Run - Production' />
var path = require('path')
const VueLoaderPlugin = require('vue-loader/lib/plugin'); // плагин для загрузки кода Vue

module.exports = {
  entry: './Client/main.js',
  output: {
    path: path.resolve(__dirname, './wwwroot/js'),
    publicPath: '/wwwroot/js/',
    filename: 'build.js'
  },
  module: {
    rules: [
      {
        test: /\.vue$/,
        loader: 'vue-loader'
      }, {
        test: /\.css$/,
        use: [
          'vue-style-loader',
          'css-loader'
        ]
      }, {
        test: /\.(png|jpg|gif)$/,
        use: [
          {
            loader: 'file-loader',
            options: {},
          },
        ],
      },
    ]
  },
  plugins: [
    new VueLoaderPlugin()
  ],
  devServer: {
    contentBase: path.join(__dirname, 'wwwroot'),
    publicPath: '/js/',
    historyApiFallback: {
      index: 'index.html'
    },
    watchContentBase: true,
    compress: true
  }
}