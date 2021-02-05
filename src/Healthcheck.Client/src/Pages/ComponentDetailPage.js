import React from 'react';
import { useParams } from "react-router-dom";
import { useLocation, useNavigate } from "react-router-dom";
import TableView from '../Components/TableView';
import ScApplicationHeader from '../ScComponents/ScApplicationHeader';
import ScIcon from '../ScComponents/ScIcon';
import ScProgressIndicatior from '../ScComponents/ScProgressIndicatior';

class ComponentDetailPage extends React.Component {  
  constructor(props){
    super(props)
    
  }

  componentDidMount () {

    console.log( this.props);
}

  render() {
    return (
     <div>
       <h1>detail page: todo, implement something nice here {this.props.params.id} </h1>
      </div>
    );
  }
}


// Hacking react router v6
export default withRouter(ComponentDetailPage);

export function withRouter( Child ) {
  return ( props ) => {
    const location = useLocation();
    const navigate = useNavigate();
    const params = useParams();
    return <Child { ...props } navigate={ navigate } location={ location } params={params} />;
  }
}
