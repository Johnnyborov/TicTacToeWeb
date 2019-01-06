<script>
export default {
  props: {
    value: {
      type: Object,
      default: {type: '', status: 'normal'}
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

    let xDim = this.$store.state.gameEntity.xDim
    let yDim = this.$store.state.gameEntity.yDim
    let maxDim = Math.max(xDim,yDim)

    let halfCellBorderWidth = Math.round(ctx.canvas.width / 25 / maxDim)
    let cellBorderWidth = 2 * halfCellBorderWidth
    let imgSize = Math.round((ctx.canvas.width - cellBorderWidth*(maxDim * 3)) / maxDim)
    let imgStep = (imgSize + 3 * cellBorderWidth)

    ctx.lineWidth = cellBorderWidth

    let i = this.i
    let j = this.j
    
    if (this.value.status == 'pressed')
      ctx.strokeStyle = '#08F'
    else if  (this.value.status == 'disabled')
      ctx.strokeStyle = '#444'
    else if  (this.value.status == 'chain-member')
      ctx.strokeStyle = '#080'
    else if  (this.value.status == 'normal')
      ctx.strokeStyle = '#048'
    else
      throw "invalid cell status"

    ctx.strokeRect(2*halfCellBorderWidth+imgStep*j, 2*halfCellBorderWidth+imgStep*i, imgSize+2*halfCellBorderWidth, imgSize+2*halfCellBorderWidth)         

    let cellImg;
    switch(this.value.type) {
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
        throw "Invalid cell type"
    }

    ctx.drawImage(cellImg, halfCellBorderWidth + cellBorderWidth+imgStep*j, halfCellBorderWidth + cellBorderWidth+imgStep*i, imgSize, imgSize)
  }
}
</script>