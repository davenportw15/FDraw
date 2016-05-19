open System
open System.Drawing

// Define Triangle type 
type Top = Point
type Left = Point
type Right = Point
type Triangle = Top * Left * Right

// Draw filled triangle with brush
let drawTriangle (graphics : Graphics) (brush : Brush) (triangle : Triangle) =
    let (top, left, right) = triangle
    let points = [| top; left; right |]
    graphics.FillPolygon (brush, points)

// Creates a triangle from a rectangle (equilateral from square)
let triangleFromRectangle (rectangle : Rectangle) : Triangle =
    let top   = new Point (rectangle.Left + rectangle.Width / 2, rectangle.Top)
    let left  = new Point (rectangle.Left, rectangle.Bottom)
    let right = new Point (rectangle.Right, rectangle.Bottom)
    (top, left, right)

// Divides a triangle into three main subtriangles and one inverted triangle
let divideTriangle (triangle : Triangle) : Triangle * Triangle * Triangle * Triangle = 

    // Calculates midpoint between 2 points
    let midpoint (point1 : Point) (point2 : Point) = 
        let x = (point1.X + point2.X) / 2
        let y = (point1.Y + point2.Y) / 2
        new Point (x, y)

    // Vertices of triangle
    let (top, left, right) = triangle

    // Midpoints
    let leftMidpoint   = midpoint top left
    let bottomMidpoint = midpoint left right
    let rightMidpoint  = midpoint top right

    // Subtriangles
    let topTriangle      = (top, leftMidpoint, rightMidpoint)
    let leftTriangle     = (leftMidpoint, left, bottomMidpoint)
    let rightTriangle    = (rightMidpoint, bottomMidpoint, right)
    let invertedTriangle = (leftMidpoint, bottomMidpoint, rightMidpoint)

    (topTriangle, leftTriangle, rightTriangle, invertedTriangle)

// Draws triangles and all subtriangles in fractal; depth specified in 
let rec drawSubTriangles (numberOfTimes : int) (graphics : Graphics) (mainBrush : Brush) (invertedBrush : Brush) (triangle : Triangle) = 
    if numberOfTimes > 0 then 
        let (top, left, right, inverted) = divideTriangle triangle
        let mainTriangles = [top; left; right]

        // Draw main triangles
        for triangle' in mainTriangles do
            drawTriangle graphics mainBrush triangle'

        // Draw inverted triangle
        drawTriangle graphics invertedBrush inverted

        // Recur
        let draw = drawSubTriangles (numberOfTimes - 1) graphics mainBrush invertedBrush
        for triangle' in mainTriangles do
            draw triangle'


[<EntryPoint>]
let main argv = 

    // Constants:
    // Change according to your specification
    // Make sure to change the outputPath!
    let outputPath      = "/Users/william/Desktop/fractal.bmp"
    let height          = 10000
    let width           = 10000
    let iterations      = 10
    let mainColor       = Color.Blue         // Color of main triangles
    let invertedColor   = Color.DarkSeaGreen // Color of inverted triangle
    let backgroundColor = Color.White        // Color of background
    let drawBackground  = false              // Enable if 

    // Graphics
    let bitmap   = new Bitmap (height, width)
    let graphics = Graphics.FromImage (bitmap)

    // Bounds
    let background      = new Rectangle (0, 0, height, width)

    // Draw background (if drawBackground is true)
    if drawBackground then
        let backgroundBrush = new SolidBrush (backgroundColor)
        graphics.FillRectangle (backgroundBrush, background)

    // Brushes
    let mainBrush     = new SolidBrush (mainColor)
    let invertedBrush = new SolidBrush (invertedColor)

    // Outermost triangle
    let mainTriangle  = triangleFromRectangle background
 
    // Initiate drawing with specified number of iteration
    drawSubTriangles iterations graphics mainBrush invertedBrush mainTriangle

    // Save bitmap
    bitmap.Save (outputPath)

    // Return success
    0