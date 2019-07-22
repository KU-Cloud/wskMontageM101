const PORT = 5122
const REPO = 'files'

const express = require('express')
const multer = require('multer')

const upload = multer({
    storage: multer.diskStorage({
        destination: (req, file, cb) => {
            cb(null, REPO)
        },
        filename: (req, file, cb) => {
            cb(null, file.fieldname)
        }
    })
})
const app = express()

app.use('/files', express.static(REPO))

app.post('/files', upload.any(), (req, res) => {
    for (let file of req.files) {
        console.log(file.filename)
    }

    res.send('uploaded');
})

app.listen(PORT, () => {
    console.log('listening...')
});
