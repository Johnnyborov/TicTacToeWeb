<template>
<div id="game-field">
  <canvas id="canvas" ref="canvas"
  @mousedown.prevent="$emit('mouse-down', getCellIndex($event))"
  @mouseup="$emit('mouse-up', getCellIndex($event))"
  @mouseleave="$emit('mouse-leave')"
  @touchstart.prevent="touchStartHandler($event)"
  @touchend="touchEndHandler($event)">
  </canvas>
</div>
</template>

<script>
import Resources from '../resources/resources.js'

function getSizeAttributes(context,state) {
  let xDim = state.xDim
  let yDim = state.yDim
  let maxDim = Math.max(xDim,yDim)

  let cellBorderWidth = Math.floor(context.canvas.width / 12 / maxDim)
  if (cellBorderWidth === 0) {
    cellBorderWidth = 1
  }
  let imgSize = Math.floor((context.canvas.width - (maxDim*2 + maxDim-1)*cellBorderWidth) / maxDim)
  let imgStep = (imgSize + 3*cellBorderWidth)

  return {xDim: xDim, yDim: yDim, cellBorderWidth: cellBorderWidth, imgSize: imgSize, imgStep: imgStep}
}

export default {
  data() {
    return {
      context: null,

      clearImg: null,
      crossImg: null,
      noughtImg: null,

      lastTouch: null
    }
  },

  created() {
    window.addEventListener("resize", this.resizeHandler)
    window.addEventListener('load', this.loadedHandler)
  },
  
  destroyed() {
    window.removeEventListener("resize", this.resizeHandler)
    window.removeEventListener('load', this.loadedHandler)
  },

  mounted() {
    this.context = this.$refs['canvas'].getContext('2d')

    this.$refs['canvas'].width = this.$refs['canvas'].parentElement.clientWidth
    this.$refs['canvas'].height = this.$refs['canvas'].parentElement.clientHeight

    this.clearImg = new Image()
    this.clearImg.src = Resources.clear

    this.crossImg = new Image()
    this.crossImg.src = Resources.cross

    this.noughtImg = new Image()
    this.noughtImg.src = Resources.nought
  },

  methods: { 
    touchStartHandler(e) {
      // deactivate prevoius mouse-down if more than one touch happens simultaneously
      if (e.touches.length > 1) {
        this.$emit('mouse-up', -1)
      }

      // handle new one
      this.$emit('mouse-down', this.getCellIndex(e.touches[e.touches.length-1]))
      this.lastTouch = e.touches[e.touches.length-1]
    },

    touchEndHandler(e) {
      if (this.lastTouch === null) return

      // check if lastTouch got removed from touches list (meaning that's the event that's handling lastTouch)
      let removed = true
      for (let i = 0; i < e.touches.length; i++) {
        if (e.touches[i].identifier === this.lastTouch.identifier) removed = false
      }

      if (removed) {
        this.$emit('mouse-up', this.getCellIndex(this.lastTouch))
        this.lastTouch = null
      }
    },

    resizeHandler: function() {
      this.$refs['canvas'].width = this.$refs['canvas'].parentElement.clientWidth
      this.$refs['canvas'].height = this.$refs['canvas'].parentElement.clientHeight

      this.drawAllCells()
    },

    loadedHandler: function() {
      this.drawAllCells()
    },

    drawAllCells: function() {
      this.context.clearRect(0,0,this.context.canvas.width,this.context.canvas.height)

      for (let index = 0; index < this.$store.state.gameEntity.cells.length; index++) {
        this.drawCell(index)
      }
    },

    drawCell(index) {
      if (!this.context) return
      let ctx = this.context

      let sizes = getSizeAttributes(ctx, this.$store.state.gameEntity)

      let i = Math.floor(index / sizes.xDim)
      let j = index % sizes.xDim

      let cell = this.$store.state.gameEntity.cells[index]

      if (cell.status == 'pressed')
        ctx.strokeStyle = '#08F'
      else if  (cell.status == 'disabled')
        ctx.strokeStyle = '#444'
      else if  (cell.status == 'chain-member')
        ctx.strokeStyle = '#080'
      else if  (cell.status == 'normal')
        ctx.strokeStyle = '#048'
      else
        throw "invalid cell status"

      ctx.lineWidth = sizes.cellBorderWidth

      ctx.strokeRect(sizes.cellBorderWidth/2+sizes.imgStep*j, sizes.cellBorderWidth/2+sizes.imgStep*i,
                    sizes.imgSize+sizes.cellBorderWidth, sizes.imgSize+sizes.cellBorderWidth)

      let cellImg;
      switch(cell.type) {
        case 'clear':
          cellImg = this.clearImg
          break
        case 'cross':
          cellImg = this.crossImg
          break
        case 'nought':
          cellImg = this.noughtImg
          break
        default:
          throw "Invalid cell type"
      }

      ctx.drawImage(cellImg, sizes.cellBorderWidth+sizes.imgStep*j, sizes.cellBorderWidth+sizes.imgStep*i,
                    sizes.imgSize, sizes.imgSize)
    },

    getCellIndex: function(e) {
      let sizes = getSizeAttributes(this.context, this.$store.state.gameEntity)

      let rect = this.context.canvas.getBoundingClientRect()
      let x = e.clientX - rect.left
      let y = e.clientY - rect.top

      let i = Math.floor(y / sizes.imgStep)
      let j = Math.floor(x / sizes.imgStep)

      if (i >= sizes.yDim || j >= sizes.xDim)
        return -1

      if (y%sizes.imgStep < sizes.CellBorderWidth/2 ||
          (sizes.imgStep - y%sizes.imgStep) < sizes.CellBorderWidth/2 ||
          x%sizes.imgStep < sizes.CellBorderWidth/2 ||
          (sizes.imgStep - x%sizes.imgStep) < sizes.CellBorderWidth/2)
        return -1

      return i*sizes.xDim + j
    }
  }
}
</script>

<style>
  #game-field {
    background: burlywood;
    width: 100%;
    height: 70%;
    position: absolute;
  }

  @media (orientation: landscape) {
    #game-field {
      width: 70%;
      height: 100%;
    }
  }
</style>
