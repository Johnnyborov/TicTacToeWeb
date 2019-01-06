<script>
export default {
  props: {
    value: {
      type: Object,
      default: {status: '', pressed: false}
    },

    i: {
      type: Number,
      default: -1
    },

    j: {
      type: Number,
      default: -1
    }
  },

  render() {
    if (!this.$parent.context) return
    let ctx = this.$parent.context
console.log('render')
    let halfCellBorderWidth = Math.round(ctx.canvas.width / 40 / 2)
    let cellBorderWidth = 2 * halfCellBorderWidth
    let imgSize = Math.round((ctx.canvas.width - cellBorderWidth*(3 * 2 + 3)) / 3)
    let imgStep = (imgSize + 3 * cellBorderWidth)

    ctx.lineWidth = cellBorderWidth

    let i = this.i
    let j = this.j
    
    if (this.value.pressed)
      ctx.strokeStyle = '#08F'
    else
      ctx.strokeStyle = '#048'

    ctx.strokeRect(2*halfCellBorderWidth+imgStep*j, 2*halfCellBorderWidth+imgStep*i, imgSize+2*halfCellBorderWidth, imgSize+2*halfCellBorderWidth)         

    let cellImg;
    switch(this.value.status) {
      case 'clear':
        cellImg = this.$parent.clearImg
        break
      case 'cross':
        cellImg = this.$parent.crossImg
        break
      case 'nought':
        cellImg = this.$parent.noughtImg
        break
      default:
        throw "Invalid cell value"
    }

    ctx.drawImage(cellImg, halfCellBorderWidth + cellBorderWidth+imgStep*j, halfCellBorderWidth + cellBorderWidth+imgStep*i, imgSize, imgSize)
  }
}
</script>