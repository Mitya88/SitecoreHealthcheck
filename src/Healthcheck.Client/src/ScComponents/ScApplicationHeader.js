import React from 'react';

class ScApplicationHeader extends React.Component {
    // Used from Sitecore SPEAK3 library
    render() {
        return (
            <div className="page-app-header">
                <header className="app-header">
                    <div className="app-header-bg">
                        <svg height="90px" version="1.1" viewBox="0 0 1920 90" width="1920px" xmlns="http://www.w3.org/2000/svg">
                            <defs>
                                <linearGradient id="linearGradient-1" x1="100%" x2="0.697835286%" y1="50%" y2="50%">
                                    <stop offset="0%" stopColor="#CA241C"></stop>
                                    <stop offset="100%" stopColor="#C52425"></stop>
                                </linearGradient>
                                <linearGradient id="linearGradient-2" x1="100%" x2="0.697835286%" y1="50%" y2="50%">
                                    <stop offset="0%" stopColor="#C8231D"></stop>
                                    <stop offset="100%" stopColor="#C21C22" stopOpacity="0"></stop>
                                </linearGradient>
                                <path d="M154.077853,28.220459 C162.344129,22.8741048 222.599174,19.1339518 334.842989,17 L278.155489,134.900391 C120.595594,188.717448 50.3843306,197.314779 67.5217004,160.692383 C93.2277551,105.758789 141.678439,36.2399902 154.077853,28.220459 Z" id="path-3"></path>
                                <polygon id="path-4" points="400 24 546.860352 24 577.553711 137.555664 474.618164 133.599609"></polygon>
                                <polygon id="path-5" points="514.874023 21.0224609 818.740234 0.18359375 881.626953 136.478516 580.918945 135.384766"></polygon>
                                <polygon id="path-6" points="890.15918 25.9560547 967.396484 32 1085.03027 126.822266 992.761719 130.300781"></polygon>
                                <path d="M769,15.5085022 C848.630208,13.2425704 896.097005,18.5495366 911.400391,31.4294006 C934.355469,50.7491966 967.233398,79.5323998 1003.05957,132.696979 C1026.94368,168.140031 965.088542,168.140031 817.494141,132.696979 L769,15.5085022 Z" id="path-7"></path>
                            </defs>
                            <g fill="none" fillRule="evenodd" id="Page-1" stroke="none" strokeWidth="1">
                                <g id="asset_applicationheader_bg">
                                    <g id="Group" transform="translate(0.000000, -32.000000)">
                                        <rect fill="url(#linearGradient-1)" height="90" id="Rectangle-3" width="1920" x="0" y="32"></rect>
                                        <rect fill="url(#linearGradient-2)" height="90" id="Rectangle-3-Copy" width="799" x="1121" y="32"></rect>
                                        <rect height="93" id="Rectangle-4" stroke="#CB3327" transform="translate(335.325483, 50.594040) rotate(50.000000) translate(-335.325483, -50.594040) " width="1" x="335.325483" y="4.09403988"></rect>
                                        <polygon fill="#C01920" id="Rectangle-5" points="545.086914 69.5541992 579.658691 122 509.064941 122"></polygon>
                                        <g id="Rectangle-Copy-5">
                                            <use fill="#D73127" fillRule="evenodd"></use>
                                            <path d="M277.795646,134.49493 L334.040394,17.5153886 C222.578009,19.647044 162.467609,23.3897057 154.349392,28.6402999 C148.265059,32.5754453 133.128428,51.8221658 116.205463,77.1414908 C97.5963833,104.983499 79.8751621,135.472849 67.9745694,160.904302 C59.6478274,178.69847 72.4801616,185.474438 107.266349,181.214888 C142.29924,176.92513 199.148976,161.351488 277.795646,134.49493 Z" stroke="#E23629" strokeWidth="1"></path>
                                        </g>
                                        <g id="Rectangle-Copy-2" transform="translate(488.776855, 80.777832) scale(-1, 1) translate(-488.776855, -80.777832) ">
                                            <use fill="#C42224" fillRule="evenodd"></use>
                                            <path d="M400.945293,24.5 L474.88948,133.109668 L576.893665,137.029928 L546.477556,24.5 L400.945293,24.5 Z" stroke="#CB3327" strokeWidth="1"></path>
                                        </g>
                                        <g id="Rectangle-Copy-3">
                                            <use fill="#C42224" fillRule="evenodd"></use>
                                            <path d="M515.707821,21.4664542 L581.208187,134.885814 L880.84428,135.975666 L818.430617,0.706001393 L515.707821,21.4664542 Z" stroke="#CB3327" strokeWidth="1"></path>
                                        </g>
                                        <g id="Rectangle-Copy-4" opacity="0.5">
                                            <use fill="#CE2D27" fillRule="evenodd"></use>
                                            <path d="M891.453126,26.5588366 L992.963467,129.79282 L1083.67616,126.37296 L967.203187,32.4864026 L891.453126,26.5588366 Z" stroke="#D02F26" strokeWidth="1"></path>
                                        </g>
                                        <g id="Rectangle-Copy">
                                            <use fill="#D73127" fillRule="evenodd"></use>
                                            <path d="M769.739471,15.9878288 L817.858692,132.270293 C891.489298,149.943508 943.738479,158.779268 974.581299,158.779268 C1005.17602,158.779268 1014.29615,150.266324 1002.64493,132.976394 C986.228562,108.615174 969.5121,87.9605676 952.042195,69.7172114 C938.947837,56.0431228 927.463671,45.602338 911.078428,31.8119448 C895.957544,19.0856805 848.791536,13.7787391 769.739479,15.9878471 L769.739471,15.9878288 Z" stroke="#E23629" strokeWidth="1"></path>
                                        </g>
                                    </g>
                                </g>
                            </g>
                        </svg>
                    </div>
                    <div className="app-header-logo">
                        <svg height="90px" version="1.1" viewBox="0 0 109 90" width="109px" xmlns="http://www.w3.org/2000/svg">
                            <defs></defs>
                            <g fill="none" fillRule="evenodd" id="Page-2" stroke="none" strokeWidth="1">
                                <g fill="#A21D16" fillRule="nonzero" id="asset_applicationheader_logo">
                                    <g transform="translate(0.000000, -98.000000)">
                                        <path d="M93,0.5 C41.6,0.5 0,42.4 0,94 C0,145.6 41.6,187.5 93,187.5 C144.4,187.5 186,145.6 186,94 C186,42.4 144.4,0.5 93,0.5 Z M93.9,161.8 C56.3,161.8 25.9,131.2 25.9,93.4 C25.9,55.6 56.3,25 93.9,25 C131.5,25 161.9,55.6 161.9,93.4 C161.9,131.2 131.5,161.8 93.9,161.8 Z"></path>
                                        <path d="M30.8,110.8 C30.8,110.8 41.9,154.3 85.5,159 C129,163.7 150.8,125.8 150.8,125.8 L139.1,118.6 C139.1,118.6 121,149.2 88.6,148.1 C56.2,147.1 39.9,127.7 30.8,110.8 Z"></path>
                                        <path d="M53.7,135.4 C63.5,142.4 75.1,146 88.6,146.5 C119.6,147.5 137.5,118.1 137.7,117.8 L138.6,116.4 L140,117.3 L150.9,124.1 C160.9,107.6 158.5,89.2 158.5,89.2 L138.3,89.4 C138.3,89.4 139.9,119.2 104.1,136 C89.2,143 64.2,141.9 49.2,130 C49.2,130 50.5,132.4 53.7,135.4 Z"></path>
                                        <path d="M135.4,87.8 L135.3,86.1 L137,86.1 L158.2,85.9 C157.3,69.4 144.8,54.9 144.8,54.9 L121,69.7 C121,69.7 136.2,81 122.4,107.8 C117.6,117.1 107.3,127.2 97.9,131.6 C86.4,137 77.5,137.3 77.5,137.3 C78.7,137.8 89.8,140 105.5,133 C136.4,119.3 135.4,88.9 135.4,87.8 Z"></path>
                                    </g>
                                </g>
                            </g>
                        </svg>
                    </div>
                    <div className="app-header-content">
                        <h3 className="app-header-text">{this.props.title}</h3>
                        <h5 className="basic-white">{this.props.subTitle}</h5>
                    </div>
                </header>
            </div>
        );
    }
}

export default ScApplicationHeader;
