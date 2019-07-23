const PORT = 5122
const REPO = 'files'

const fs = require('fs')
const path = require('path')

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

let total = 0;


app.use(function (req, res, next) {
    if (req.method === "GET") {
        const filename = path.basename(req.url);
        const stat = fs.statSync(path.join(REPO, filename))
        const fileSizeInBytes = stat.size;
        console.log(`${filename} ${fileSizeInBytes} ${total += fileSizeInBytes}`)
    }
    next();
});
app.use('/files', express.static(REPO))

app.post('/files', upload.any(), (req, res) => {
    for (let file of req.files) {
        const fileSizeInBytes = file.size;
        console.log(`${file.filename} ${fileSizeInBytes} ${total += fileSizeInBytes}`)
    }

    res.send('uploaded');
})

app.listen(PORT, () => {
    console.log('listening...')
});
