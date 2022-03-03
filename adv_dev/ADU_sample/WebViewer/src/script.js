import './style.css'
import * as THREE from 'three'
import { IFCLoader } from "web-ifc-three/IFCLoader"
import { OrbitControls } from 'three/examples/jsm/controls/OrbitControls'
import { ACESFilmicToneMapping, AmbientLight, Color, DirectionalLight, Raycaster, Vector2 } from 'three'
import { acceleratedRaycast, computeBoundsTree, disposeBoundsTree } from "three-mesh-bvh"




// Canvas
const width = document.documentElement.clientWidth
const height = document.documentElement.clientHeight
const canvas = document.querySelector("canvas.canvas")
const output = document.getElementsByClassName("output")[0]
const size = {
    width: width,
    height: height
}
const lines = {
    pt1: null,
    pt2: null,
    pt3: null
}
const initialDist = 100
const topPanelCoord = 0.75


// Scene
const scene = new THREE.Scene()

// Interactive Viewport Size
window.addEventListener("resize", (e) => {
    size.width = document.documentElement.clientWidth
    size.height = document.documentElement.clientHeight
    // update camera
    camera.aspect = size.width / size.height;
    camera.updateProjectionMatrix();
    // update render
    renderer.setSize(size.width, size.height);
    console.log(size)
})


// // Full screen
// window.addEventListener("dblclick", (e) => {
//     if (!document.fullscreenElement) {
//         webScreen.requestFullscreen()
//         console.log("enter fullscreen!")
//     }
//     else {
//         webScreen.exitFullscreen()
//         console.log("exit fullscreen!")
//     }
// })

// Geometry
const platform = new THREE.Mesh(new THREE.BoxGeometry(55, 55, -1), new THREE.MeshBasicMaterial("white")).rotateX(-Math.PI / 2)
platform.position.set(-46, -2, 0)
scene.add(platform)

// Material
const preselectMat = new THREE.MeshLambertMaterial(
    {
        transparent: true,
        opacity: 0.6,
        color: "#ff948c",
        depthTest: false
    }
)
const selecteMat = new THREE.MeshLambertMaterial(
    {
        transparent: true,
        opacity: 1,
        color: "#ff948c",
        depthTest: true
    }
)


// lights
const lightColor = 0xffffff
const ambientLight = new AmbientLight(lightColor, 0.5)
scene.add(ambientLight)
const directLight = new DirectionalLight(lightColor, 1)
directLight.position.set(0, initialDist, 0)
directLight.target.position.set(-5, 0, 0)
scene.add(directLight)
scene.add(directLight.target)

// Camera
const camera = new THREE.PerspectiveCamera(45, size.width / size.height, 0.1, 1000)
camera.position.x = initialDist
camera.position.y = initialDist
camera.position.z = initialDist
// Renderer
const renderer = new THREE.WebGLRenderer({
    canvas: canvas,
    alpha: true
})
renderer.setSize(size.width, size.height)

// Orbit Control
const control = new OrbitControls(camera, canvas)
control.dynamicDampingFactor = 0.1

// Animation Loop
const animate = () => {
    control.update()
    renderer.render(scene, camera)
    window.requestAnimationFrame(animate)
}


animate()

// picking
const ifcModels = []
const raycast = new Raycaster()
raycast.firstHitOnly = true
const mouse = new Vector2()

const cast = (e) => {
    const bounds = canvas.getBoundingClientRect()

    const x1 = e.clientX - bounds.left
    const x2 = bounds.right - bounds.left
    mouse.x = (x1 / x2) * 2 - 1

    const y1 = e.clientY - bounds.top
    const y2 = bounds.bottom - bounds.top
    mouse.y = -(y1 / y2) * 2 + 1

    raycast.setFromCamera(mouse, camera)

    return raycast.intersectObjects(ifcModels)
}


// Load IFC
const ifcLoad = new IFCLoader()
const input = document.getElementById("fileInput")
input.addEventListener(
    "change",
    (e) => {
        const file = e.target.files[0]
        var fileURL = URL.createObjectURL(file)
        ifcLoad.load(
            fileURL,
            (ifcModel) => {
                ifcModels.push(ifcModel)
                scene.add(ifcModel)
            }

        )
    },
    false
)
console.log(ifcLoad)
ifcLoad.ifcManager.setWasmPath("wasm/")



// text
function createText(text, pos) {
    const fontLoader = new THREE.FontLoader().load(
        '/fonts/Roboto Light_Regular.json',
        (font) => {
            let textGeo = new THREE.Mesh(new THREE.TextGeometry(
                text, {
                font: font,
                size: 0.012,
                height: 0.0,
                curveSegments: 30,
                bevelEnabled: false,
                bevelThickness: 0.15,
                bevelSize: 0.15,
                bevelOffset: 0,
                bevelSegments: 1
            }
            ), new THREE.MeshBasicMaterial({
                color: "black"
            }))
            textGeo.depthTest = false
            console.log("loaded font!")
            camera.add(textGeo)
            textGeo.position.set(pos.x, pos.y, -1)
        }
    )
}

// reaction from picking
let preselectModel = { id: -1 }
const ifc = ifcLoad.ifcManager

const pick = (e, material, model, hasdata) => {
    const found = cast(e)[0]
    if (found) {

        const index = found.faceIndex
        const geo = found.object.geometry

        const expressId = ifc.getExpressId(geo, index)
        model.id = found.object.modelID
        //create subset
        ifc.createSubset(
            {
                scene: scene,
                modelID: model.id,
                ids: [expressId],
                removePrevious: true,
                material: material
            }
        )

        if (hasdata) {
            console.log(found)
            const type = ifc.getItemProperties(ifcModels[0].modelID, expressId)
            let text = type.Name["value"]
            output.innerHTML = text
            scene.add(camera)
            camera.children = []
            createText(text, new THREE.Vector3(0.35, 0.41, -1))
        }

    }
    else {

        ifc.removeSubset(model.id, scene, material)
        if (hasdata) {
            camera.children = []
            output.innerHTML = "Elements will appear here!"
        }

    }
}

const home = document.getElementsByClassName("label")[0]
home.addEventListener(
    "mouseover",
    (e) => {
        home.style.color = "red"
    }
)
home.addEventListener(
    "mouseout",
    (e) => {
        home.style.color = "black"
    }
)

window.onmousemove = (e) => pick(
    e, preselectMat, preselectModel, false
)
window.ondblclick = (e) => pick(
    e, selecteMat, preselectModel, true
)









