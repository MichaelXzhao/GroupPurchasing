const routes = [
    {
        path:'/test',
        component:() => import('../pages/test.vue')
    },
    {
        path:"/createGroupBuy",
        component:()=>import("../pages/createGroupBuy.vue")
    },
    {
        path:"/joinGroupBuy",
        component:()=>import("../pages/joinGroupBuy.vue")
    },
    {
        path:"/createGroupBuy/hostGroupBuy",
        component:()=>import("../pages/host/hostGroupBuy.vue")
    },
    {
        path:"/joinGroupBuy/memberGroupBuy",
        component:()=>import("../pages/member/memberGroupBuy.vue")
    },
    {
        path:"/result",
        component:()=>import("../pages/result.vue")
    },
    {
        path:"/profile",
        component:()=>import("../pages/profile.vue")
    },
    {
        path:"/dashboard",
        component:()=>import("../pages/host/hostDashBoard.vue")
    }
]

export default routes ;