const cmps = require('openwhisk-composer')

// sorry for the hard-coded
// but i don't have enough time to learn composer

module.exports = cmps.sequence(
    cmps.action('Step1'),
    cmps.action('Step2'),
    cmps.parallel(
        cmps.function((args) => { return args; }),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['projTasks'][0] } }), cmps.action('Step3')),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['projTasks'][1] } }), cmps.action('Step3')),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['projTasks'][2] } }), cmps.action('Step3')),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['projTasks'][3] } }), cmps.action('Step3')),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['projTasks'][4] } }), cmps.action('Step3')),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['projTasks'][5] } }), cmps.action('Step3')),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['projTasks'][6] } }), cmps.action('Step3')),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['projTasks'][7] } }), cmps.action('Step3')),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['projTasks'][8] } }), cmps.action('Step3')),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['projTasks'][9] } }), cmps.action('Step3'))
    ),
    cmps.function((args) => {
        var rst = args.value;
        var org = rst.shift();

        var logs = [];
        for (const val of rst) {
            logs.push(val.log);
        }

        org.projected = [];
        for (const tasks of org.projTasks) {
            org.projected.push(tasks.dst);
        }
        delete org.projTasks;

        org.log.push(logs);
        return org;
    }),
    cmps.action('Step4'),
    cmps.action('Step5'),
    cmps.action('Step6'),
    cmps.parallel(
        cmps.function((args) => { return args; }),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['diffTasks'][0] } }), cmps.action('Step7')),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['diffTasks'][1] } }), cmps.action('Step7')),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['diffTasks'][2] } }), cmps.action('Step7')),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['diffTasks'][3] } }), cmps.action('Step7')),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['diffTasks'][4] } }), cmps.action('Step7')),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['diffTasks'][5] } }), cmps.action('Step7')),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['diffTasks'][6] } }), cmps.action('Step7')),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['diffTasks'][7] } }), cmps.action('Step7')),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['diffTasks'][8] } }), cmps.action('Step7')),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['diffTasks'][9] } }), cmps.action('Step7')),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['diffTasks'][10] } }), cmps.action('Step7')),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['diffTasks'][11] } }), cmps.action('Step7')),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['diffTasks'][12] } }), cmps.action('Step7')),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['diffTasks'][13] } }), cmps.action('Step7')),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['diffTasks'][14] } }), cmps.action('Step7')),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['diffTasks'][15] } }), cmps.action('Step7')),
        cmps.sequence(cmps.function((args) => { return { 'serverURL': args['serverURL'], 'task': args['diffTasks'][16] } }), cmps.action('Step7'))
    ),
    cmps.function((args) => {
        var rst = args.value;
        var org = rst.shift();

        var logs = [];
        for (const val of rst) {
            logs.push(val.log);
        }

        org.diffs = [];
        for (const tasks of org.diffTasks) {
            org.diffs.push(tasks.dst);
        }
        delete org.diffTasks;

        org.log.push(logs);
        return org;
    }),
    cmps.action('Step8'),
    cmps.action('Step9'),
    cmps.action('Step10'),
    cmps.action('Step11')
);
