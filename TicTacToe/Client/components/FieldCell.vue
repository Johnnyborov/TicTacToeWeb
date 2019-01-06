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

    let halfCellBorderWidth = Math.floor(ctx.canvas.width / 25 / maxDim)
    let cellBorderWidth = 2 * halfCellBorderWidth
    if (halfCellBorderWidth === 0) {
      cellBorderWidth = 1
    }
    let imgSize = Math.floor((ctx.canvas.width - maxDim*3*cellBorderWidth) / maxDim)
    let imgStep = (imgSize + 3*cellBorderWidth)

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

    ctx.strokeRect(cellBorderWidth+imgStep*j, cellBorderWidth+imgStep*i, imgSize+cellBorderWidth, imgSize+cellBorderWidth)         

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